using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    ///测试完成

    /// <summary>
    /// 对朋友的响应
    /// </summary>
    public class FriendRepository : Repository<Friend, EaDbContext>
    {
        private readonly UserManager<EaUser> _userManager;
        private readonly EaDbContext _dbContext;
        private readonly IOptions<WebApiSettings> _settings;
        public FriendRepository(UserManager<EaUser> userManager,
            EaDbContext dbContext,
            IOptions<WebApiSettings> settings)
            :base(dbContext,dbContext.Friends)
        {
            _userManager = userManager;
            _settings = settings;
            _dbContext = dbContext;
        }

        public override Task AutoFillAsync(IEnumerable<Friend> entities)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="searchName">名字包含字符</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<UserRepositoryModel> SearchFirendAsync(string searchName, int skip, int take)
        {
            if (string.IsNullOrEmpty(searchName))
            {
                return new UserRepositoryModel() { Ok = false };
            }
            var uss = _dbContext.Users.ToAsyncEnumerable()
                                      .Where(u => u.UserName.Contains(searchName))
                                      .Select(u=> new { Name = u.UserName, u.Id });
            var count = await uss.Count();
            var datas = await uss.Skip(skip).Take(take).Select(f => new RelFriend()
            {
                Target = f.Id,
                TargetName = f.Name

            }).ToArray();
            
            var upm = new UserRepositoryModel(count, skip, take, datas);
            return upm;
        }
        public async Task<FriendRepositoryModel> GetFriendsAsync(EaUser user, int skip, int take)
        {
            var fs = NonTrackingAsyncEnum.Where(f =>  f.Status == Friend.Applied && f.IsEnable&& f.Creator == user.Id||f.Target==user.Id);
            var count = await fs.Count();
            var datas = await fs.Skip(skip).Take(take).ToArray();
            var fpm = new FriendRepositoryModel(count, skip, take, datas);
            return fpm;
        }

        /// <summary>
        /// 获取朋友
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<FriendRepositoryModel> GetFriendsAsync(string authorization,int skip, int take)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new FriendRepositoryModel() { Ok = false };
            }
            return await GetFriendsAsync(user, skip, take);
        }
        public async Task<bool> DeleteFirendAsync(EaUser user, string fuid)
        {
            var fuser = await DbSet.ToAsyncEnumerable().SingleOrDefault(f => f.IsEnable && f.Creator == user.Id && f.Target == fuid && f.Status == Friend.Applied);
            if (fuser == null)
            {
                return false;
            }
            fuser.IsEnable = false;
            DbSet.Update(fuser);
            var res = DbContext.SaveChanges();
            return res > 0;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="fuid">friend uid</param>
        /// <returns></returns>
        public async Task<bool> DeleteFirendAsync(string authorization, string fuid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await DeleteFirendAsync(user, fuid);
        }
        public async Task<bool> AcceptFriendApplyAsync(EaUser user, uint fid)
        {
            var friend = await DbSet.ToAsyncEnumerable().SingleOrDefault(f => f.Id == fid && f.IsEnable);
            Console.WriteLine($"accept f.creator={friend.Creator} f.target={friend.Target} locid={user.Id} fid={fid}");
            if (friend == null||friend.Target != user.Id)
            {
                return false;
            }
            friend.Status = Friend.Applied;
            DbSet.Update(friend);
            var res = await DbContext.SaveChangesAsync();
            return res > 0;
        }

        /// <summary>
        /// 同意朋友的申请
        /// </summary>
        /// <param name="fid">friend id</param>
        /// <returns></returns>
        public async Task<bool> AcceptFriendApplyAsync(string authorization, uint fid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await AcceptFriendApplyAsync(user, fid);
        }
        public async Task<UserRepositoryModel> GetApplyFriendsAsync(EaUser user, int s, int t)
        {
            var uss = NonTrackingAsyncEnum.Where(u => u.IsEnable && u.Status == Friend.Applying && u.Target == user.Id);
            var count = await uss.Count();
            var datas = await uss.Skip(s).Take(t).ToArray();
            var ucol = datas.Select(d => d.Target);
            var usdatas = new RelFriend[datas.Count()];
            var users = await _userManager.Users.AsNoTracking().ToAsyncEnumerable()
                                          .Where(u => ucol.Contains(u.Id))
                                          .ToDictionary(u => u.Id, u => u.UserName);

            for (int i = 0; i < datas.Count(); i++)
            {
                var odata = datas[i];
                usdatas[i] = new RelFriend()
                {
                    Id=odata.Id,
                    CreateTime = odata.CreateTime,
                    Target = odata.Target,
                    Creator = odata.Creator,
                    Status = odata.Status,
                    TargetName = users[odata.Target]
                };
            }
            var upm = new UserRepositoryModel(count, s, t, usdatas);
            return upm;
        }

        /// <summary>
        /// 获取朋友的申请
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<UserRepositoryModel> GetApplyFriendsAsync(string authorization, int s, int t)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new UserRepositoryModel() { Ok = false };
            }
            return await GetApplyFriendsAsync(user, s, t);
        }
        public async Task<bool> ApplyFriendAsync(EaUser user, string fid)
        {
            var fuser = await _userManager.FindByIdAsync(fid) ;
            if (fuser==null)
            {
                return false;//失败
            }
            var has = await AsyncEnumerable.Any(f => f.IsEnable && f.Target == user.Id || f.Creator == user.Id);
            if (fuser.Id==user.Id||has)
            {
                return false;
            }
            DbSet.Add(new Friend()
            {
                Creator = user.Id,
                Target = fid
            });
            var res =await DbContext.SaveChangesAsync();
            return res > 0;
        }
        /// <summary>
        /// 提交好友申请,不给你理由去申请o_o
        /// </summary>
        /// <param name="fid">想申请的好友id</param>
        /// <returns></returns>
        public async Task<bool> ApplyFriendAsync(string authorization, string fid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await ApplyFriendAsync(user, fid);
        }

        private async Task<EaUser> GetUserAsync(string authorization)
        {
            var token= AuthorizationHelper.ParseTokenByAuthorization(authorization, _settings.Value.SecretKey);
            if (token == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(token.sub);
        }

    }
}
