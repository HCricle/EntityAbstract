using Microsoft.EntityFrameworkCore;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.WebApi.JWT;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.WebApi.Models;

namespace SharedEA.Data.Repositories
{
    ///测试 OK

    /// <summary>
    /// 对于消息的响应
    /// </summary>
    public class MsgRepository : Repository<Msg,EaDbContext>
    {
        private readonly UserManager<EaUser> _userManager;
        private readonly FriendRepository _friendRepository;
        private readonly MsgDetailRepository _msgDetailRepository;
        private readonly IOptions<WebApiSettings> _settings;
        public MsgRepository(EaDbContext dbContext,
            IOptions<WebApiSettings> settings,
            UserManager<EaUser> userManager,
            MsgDetailRepository msgDetailRepository,
            FriendRepository friendRepository) 
            : base(dbContext,dbContext.Msgs)
        {
            _userManager = userManager;
            _msgDetailRepository = msgDetailRepository;
            _friendRepository = friendRepository;
            _settings = settings;
        }
        public override async Task AutoFillAsync(IEnumerable<Msg> msgs)
        {
            Msg msg;
            for (int i = 0; i < msgs.Count(); i++)
            {
                msg = msgs.ElementAt(i);
                msg.Details = await DbContext.MsgDetails.Where(detail => detail.MsgId == msg.Id).ToListAsync();
                //装载命令
                await _msgDetailRepository.AutoFillAsync(msg.Details);
            }
        }
        public async Task<MsgRepositoryModel> GetMsgAsync(EaUser user, int s, int t)
        {
            
            var msgs = NonTrackingAsyncEnum.Where(m => m.Creator == user.Id||m.Target==user.Id && m.IsEnable);
            var count = await msgs.Count();
            var datas = await msgs.Skip(s).Take(t).ToArray();
            var mpm = new MsgRepositoryModel(count, s, t, datas);
            return mpm;
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<MsgRepositoryModel> GetMsgAsync(string authorization, int s, int t)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new MsgRepositoryModel() { Ok = false };
            }
            return await GetMsgAsync(user, s, t);
        }
        public async Task<MsgDetailRepositoryModel> GetMsgDetailsAsync(EaUser user, uint mid, int s, int t, bool isdes = false)
        {
            var msgs = await NonTrackingAsyncEnum.SingleOrDefault(m => m.IsEnable && m.Id == mid && m.Creator == user.Id||m.Target==user.Id);

            if (msgs==null)
            {
                return new MsgDetailRepositoryModel(false);
            }
            var mds = _msgDetailRepository.Where(m => m.MsgId == msgs.Id && m.IsEnable);
            if (isdes)
            {
                mds = mds.OrderBy(m => m.Id);
            }
            else
            {
                mds = mds.OrderByDescending(m => m.Id);
            }
            var count = await mds.Count();
            var datas = await mds.Skip(s).Take(t).ToArray();
            await _msgDetailRepository.AutoFillAsync(datas);
            var mdpm = new MsgDetailRepositoryModel(count, s, t, datas);
            return mdpm;
        }

        /// <summary>
        /// 获取消息详细
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="isdes">是否倒序</param>
        /// <returns></returns>
        public async Task<MsgDetailRepositoryModel> GetMsgDetailsAsync(string authorization, uint mid,  int s,  int t,bool isdes=false)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new MsgDetailRepositoryModel(false);
            }
            return await GetMsgDetailsAsync(user, mid, s, t,isdes);
        }
        public async Task<bool> CreatMsgAsync(EaUser user, string fid)
        {
            if (user.Id==fid)
            {
                return false;
            }
            var hasFriend = await _friendRepository.NonTrackingAsyncEnum.Any(f => f.IsEnable && f.Status == Friend.Applied && f.Creator == user.Id && f.Target == fid);
            if (!hasFriend)
            {
                return false;
            }
            DbSet.Add(new Msg()
            {
                Creator = user.Id,
                Target = fid
            });
            var res = await DbContext.SaveChangesAsync();
            return res > 0;
        }

        /// <summary>
        /// 发起消息,如果已存在返回true,成功返回false
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public async Task<bool> CreatMsgAsync(string authorization, string fid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await CreatMsgAsync(user, fid);
        }
        public async Task<bool> DeleteMsgAsync(EaUser user, uint mid)
        {
            var msg = await NonTrackingAsyncEnum.SingleOrDefault(m => m.IsEnable && m.Id == mid && m.Creator == user.Id);
            if (msg == null)
            {
                return false;
            }
            msg.IsEnable = false;
            DbSet.Update(msg);
            var res = DbContext.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="mid">消息id</param>
        /// <returns></returns>
        public async Task<bool> DeleteMsgAsync(string authorization, uint mid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await DeleteMsgAsync(user, mid);
        }
        public async Task<bool> QuireNeedUpdateAsync(EaUser user, uint mid, uint lmid)
        {
            
            var msg = await NonTrackingAsyncEnum.SingleOrDefault(s => s.Id == mid);
            if (msg == null)
            {
                return false;
            }
            var rlmid = await _msgDetailRepository.Where(md => md.MsgId == msg.Id)
                                                .OrderBy(md => md.Id)
                                                .LastOrDefault();
            if (rlmid == null)//没有消息具体
            {
                return true;
            }
            return rlmid.Id > lmid;
        }

        /// <summary>
        /// 查询是否需要更新
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <param name="lmid">用户最后一条消息id</param>
        /// <returns></returns>
        public async Task<bool> QuireNeedUpdateAsync(string authorization, uint mid,  uint lmid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await QuireNeedUpdateAsync(user, mid, lmid);
        }
        public async Task<bool> SendMsgAsync(EaUser user, MsgSendModel model)
        {
            var msg = await NonTrackingAsyncEnum.SingleOrDefault(s => s.Id == model.Mid);
            if (msg == null)
            {
                return false;
            }
            _msgDetailRepository.DbSet.Add(new MsgDetail()
            {
                Content = model.Text,
                MsgId = model.Mid,
                Cmds = model.Cmd
            });
            var res = DbContext.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SendMsgAsync(string authorization,MsgSendModel model)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await SendMsgAsync(user, model);
        }

        private async Task<EaUser> GetUserAsync(string authorization)
        {
            
            var token = AuthorizationHelper.ParseTokenByAuthorization(authorization, _settings.Value.SecretKey);
            if (token == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(token.sub);
        }

    }
}
