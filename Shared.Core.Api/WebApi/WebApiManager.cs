using Newtonsoft.Json;
using Shared.Core.Api.Extensions;
using Shared.Core.Api.Models;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Core.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Shared.Core.Api.WebApi
{
    /// <summary>
    /// 对webapi的操作
    /// </summary>
    public class WebApiManager:WebApiConnecter
    {

        private static readonly string AccountUrl = "Account/";
        private static readonly string ContentUrl = "Content/";
        private static readonly string FriendUrl = "Friend/";
        private static readonly string MsgUrl = "Msg/";
        private static readonly string ShopUrl = "Shop/";
        public WebApiManager()
#if DEBUG
            :base(new Uri("http://localhost:5000/api/v1/"))
#else
            :base(new Uri("http://39.108.223.229/api/v1/"))
#endif
        {
        }
        #region From_Account
        private EaUser user;

        public EaUser User => user;

        public async Task<RelApiReponse<EaUser>> GetUserByNameAsync(string name)
        {
            var res = await GetAsync(AccountUrl + $"gubn/name={name}");
            var user = await res.GetForAsync<EaUser>();
            return new RelApiReponse<EaUser>(user,res);
        }
        public async Task<RelApiReponse<EaUser>> GetUserByIdAsync(string id)
        {
            var res = await GetAsync(AccountUrl + $"gubi/id={id}");
            var user = await res.GetForAsync<EaUser>();
            return new RelApiReponse<EaUser>(user, res);
        }
        /// <summary>
        /// 获取用户钱
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<int>> GetUserAsync(string uid)
        {
            var ap = await GetAsync(AccountUrl + $"gum/uid={uid}");
            var data = await ap.GetForAsync<int>();
            return new RelApiReponse<int>(data,ap);
        }
        /// <summary>
        /// 登陆，token会保存到此实例中，可以从只读属性AccountToken查看
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApiReponse> LoginAsync(LoginModel model)
        {
            var ar = await PostAsync(AccountUrl+"Login", false, (nameof(LoginModel.Name), model.Name), (nameof(LoginModel.Pwd), model.Pwd));
            if (ar.IsSucceed)
            {
                accountToken = await ar.GetForAsync<TokenEntity>();
                var u=await GetUserByNameAsync(model.Name);
                user = u.Data;
            }
            return ar;
        }
        
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiReponse> LogOffAsync()
        {
            accountToken = null;
            var ap = await PostAsync(AccountUrl+"LogOff");
            return ap;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApiReponse> RegisterAsync(RegisterModel model)
        {
            var ap = await PostAsync(AccountUrl+"Register", false, (nameof(RegisterModel.Email), model.Email),
                (nameof(RegisterModel.Name), model.Name), (nameof(RegisterModel.Pwd), model.Pwd));
            return ap;
        }
        public async Task<ApiReponse> ForgotPasswordAsync()
        {
            var ap = await PostAsync(AccountUrl + "fp", true);
            return ap;
        }
        #endregion
        #region From_Context
        /// <summary>
        /// <see cref="GroupRepositoryModel"/>
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        public async Task<ApiReponse> GetGroupAsync(uint gid)
        {
            var ap = await GetAsync(ContentUrl + $"gbi/gid={gid}");
            return ap;
        }
        /// <summary>
        /// 获取组的数量
        /// </summary>
        /// <returns></returns>
        public async Task<ApiReponse> GetGroupCountAsync()
        {
            var ap = await GetAsync(ContentUrl + "gc");
            return ap;
        }
        /// <summary>
        /// 获取组
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<GroupRepositoryModel>> GetGroupAsync(int take,int skip)
        {
            var ap = await GetAsync(ContentUrl + $"g/s={skip}/t={take}");
            var data = await ap.GetForAsync<GroupRepositoryModel>();
            return new RelApiReponse<GroupRepositoryModel>(data,ap);
        }
        /// <summary>
        /// 获取一个内容
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<Content>> GetSingleContentAsync(uint cid)
        {
            var ap = await GetAsync(ContentUrl + $"sct/cid={cid}");
            var data = await ap.GetForAsync<Content>();
            return new RelApiReponse<Content>(data,ap);
        }
        /// <summary>
        /// 获取用户喜欢的内容
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<ContentRepositoryModel>> GetUserLikeContentAsync(int take,int skip)
        {
            var ap = await PostAsync(ContentUrl + "ulc", true, (nameof(take), take), (nameof(skip), skip));
            var data = await ap.GetForAsync<ContentRepositoryModel>();
            return new RelApiReponse<ContentRepositoryModel>(data,ap);
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> SendContentAsync(SendContentModel model)
        {
            var ap = await PutAsync(ContentUrl+"sc", model.FileDatas, true, (nameof(model.Title), model.Title),
                (nameof(model.Content), model.Content),
                (nameof(model.Label), model.Label));
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);
        }
        /// <summary>
        /// 获取文件 获取到的是一个文件
        /// </summary>
        /// <param name="fid">文件id</param>
        /// <returns></returns>
        public async Task<ApiReponse> GetFileAsync(uint fid)
        {
            var ap = await GetAsync(ContentUrl + $"gf/fid={fid}");
            return ap;
        }
        /// <summary>
        /// 获取内容,内容自己转list<content>
        /// </summary>
        /// <param name="groupId">组id</param>
        /// <param name="page">第几页</param>
        /// <param name="autoin">是否自动组装</param>
        /// <returns></returns>
        public async Task<RelApiReponse<ContentRepositoryModel>> GetContentsAsync(uint groupId,int skip=0,int take=6,bool autoin=false)//Test
        { 
            var ap = await GetAsync(ContentUrl + $"c/gid={groupId}/s={skip}/t={take}/ai={autoin}");
            var data = await ap.GetForAsync<ContentRepositoryModel>();
            return new RelApiReponse<ContentRepositoryModel>(data,ap);
        }
        /// <summary>
        /// 获取内容具体，返回html代码
        /// </summary>
        /// <param name="cid">内容id</param>
        /// <returns></returns>
        public async Task<ApiReponse> GetContentDetailAsync(uint cid)
        {
            var ap = await GetAsync(ContentUrl + $"cd/cid={cid}");
            return ap;
        }
        /// <summary>
        /// 获取内容具体url,返回一个地址
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public string GetContentDetailUrl(uint cid)
        {
            return BaseUri + ContentUrl + $"cd/cid={cid}";
        }
        /// <summary>
        /// 搜索内容内容自己转conrepmodel
        /// </summary>
        /// <param name="searchContent">搜索内容</param>
        /// <param name="take">获取多少条</param>
        /// <param name="skip">跳过多少条</param>
        /// <param name="autoin">是否自动装载</param>
        /// <returns></returns>
        public async Task<RelApiReponse<ContentRepositoryModel>> SearchContentAsync(string searchContent, int take, int skip, bool autoin = false)
        {
            var ap = await GetAsync(ContentUrl + $"sc/ts={searchContent}/s={skip}/t={take}/ai={autoin}");
            var data = await ap.GetForAsync<ContentRepositoryModel>();
            return new RelApiReponse<ContentRepositoryModel>(data,ap);
        }
        /// <summary>
        /// 查询是否喜欢这个内容
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> IsLikeContent(uint cid)
        {
            var ap = await PostAsync(ContentUrl + "ilc", true, (nameof(cid), cid));
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);
        }
        /// <summary>
        /// 喜欢这个内容
        /// </summary>
        /// <param name="cid">内容id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> LikeContent(uint cid)
        {
            var ap = await PutAsync(ContentUrl + "lc", null, true, (nameof(cid), cid));
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);
        }
        /// <summary>
        /// 取消喜欢这个内容
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> UnLikeContent(uint cid)
        {
            var ap = await DeleteAsync(ContentUrl + $"ulc/cid={cid}", true);
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);
        }
        /// <summary>
        /// 获取内容的总数
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<int>> GetContentCountAsync(uint gid)
        {
            var ap = await GetAsync(ContentUrl + $"cc/gid={gid}");
            var data = await ap.GetForAsync<int>();
            return new RelApiReponse<int>(data,ap);
        }
        /// <summary>
        /// 获取评论，<see cref="CommentRepositoryModel"/>
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<CommentRepositoryModel>> GetCommentsAsync(uint cid,int take,int skip)
        {
            var ap = await GetAsync(ContentUrl + $"ccom/cid={cid}/t={take}/s={skip}");
            var data = await ap.GetForAsync<CommentRepositoryModel>();
            return new RelApiReponse<CommentRepositoryModel>(data,ap);
        }
        /// <summary>
        /// 发送评论
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="cstring"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> SendCommentAsync(uint cid,string cstring)
        {
            var ap = await PutAsync(ContentUrl + "scom", null, true, (nameof(cid), cid), (nameof(cstring), cstring));
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);
        }
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="cid">评论id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> DeleteCommentAsync(uint cid)
        {
            var ap = await DeleteAsync(ContentUrl + $"dcom/cid={cid}");
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);

        }
        /// <summary>
        /// 删除内容
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> DeleteContentAsync( uint cid)
        {
            var ap = await DeleteAsync(ContentUrl + $"dc/cid={cid}");
            var data = await ap.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,ap);

        }
        #endregion
        #region From_Friend
        /// <summary>
        /// 搜索用户，返回userreponsitorymodel
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<UserRepositoryModel>> SearchUserAsync(string searchName, int s, int t)
        {
            var res = await GetAsync(FriendUrl + $"su/sn={searchName}/s={s}/t={t}");
            var data = await res.GetForAsync<UserRepositoryModel>();
            return new RelApiReponse<UserRepositoryModel>(data,res);
        }
        /// <summary>
        /// 接收朋友的申请
        /// </summary>
        /// <param name="fid">朋友id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> AcceptFriendApplyAsync(uint fid)
        {
            var res = await PostAsync(FriendUrl + "afa", true, (nameof(fid), fid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 获取朋友申请列表
        /// <see cref="UserRepositoryModel"/>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<UserRepositoryModel>> GetApplyFriendsAsync(int s,int t)
        {
            var res = await PostAsync(FriendUrl + "gafs", true, (nameof(s), s), (nameof(t), t));
            var data = await res.GetForAsync<UserRepositoryModel>();
            return new RelApiReponse<UserRepositoryModel>(data,res);
        }
        /// <summary>
        /// 获取朋友列表
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<UserRepositoryModel>> GetFriendsAsync(int s,int t)
        {
            var res = await PostAsync(FriendUrl + "gf", true, (nameof(s), s), (nameof(t), t));
            var data = await res.GetForAsync<UserRepositoryModel>();
            return new RelApiReponse<UserRepositoryModel>(data, res);
        }
        /// <summary>
        /// 申请某人为好友
        /// </summary>
        /// <param name="uid">好友id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> ApplyFriendAsync(string uid)
        {
            var res = await PutAsync(FriendUrl + "af", null, true, (nameof(uid), uid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        //TODO
        #endregion
        #region From_Msg
        /// <summary>
        /// 获取会话
        /// <paramref name="s"/>
        /// <seealso cref="MsgRepositoryModel"/>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns><see cref="MsgRepositoryModel"/></returns>
        public async Task<RelApiReponse<MsgRepositoryModel>> GetMsgAsync(int s,int t)
        {
            var res = await PostAsync(MsgUrl + "gm", true, (nameof(s), s), (nameof(t), t));
            var data = await res.GetForAsync<MsgRepositoryModel>();
            return new RelApiReponse<MsgRepositoryModel>(data,res);
        }
        /// <summary>
        /// 获取消息详细
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="isdes">是否倒序</param>
        /// <returns></returns>
        public async Task<RelApiReponse<MsgDetailRepositoryModel>> GetMsgDetailAsync(uint mid, int s, int t, bool isdes = true)
        {
            var res = await PostAsync(MsgUrl + "gmds", true, (nameof(mid), mid), (nameof(s), s), (nameof(t), t),(nameof(isdes),isdes));
            var data = await res.GetForAsync<MsgDetailRepositoryModel>();
            if (data.Datas!=null)
            {
                foreach (var item in data.Datas)
                {
                    if (!string.IsNullOrEmpty(item.Cmds))
                    {
                        item.SerCmds = JsonConvert.DeserializeObject<ObservableCollection<MsgCmd>>(item.Cmds);
                    }
                }
            }
            return new RelApiReponse<MsgDetailRepositoryModel>(data,res);
        }
        /// <summary>
        /// 新建会话
        /// </summary>
        /// <param name="fid">朋友id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> CreatMsgAsync(string fid)
        {
            var res = await PutAsync(MsgUrl + "cm", null, true, (nameof(fid), fid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> DeleteMsgAsync(uint mid)
        {
            var res = await DeleteAsync(MsgUrl + $"dm/mid={mid}", true);
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 查询会话信息是否需要更新
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <param name="lmid">最后一条消息的id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> QuireNeedUpdateAsync(uint mid,uint lmid)
        {
            var res = await PostAsync(MsgUrl + "qnu", true, (nameof(mid), mid), (nameof(lmid), lmid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="model">消息模型</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> SendMsgAsync(MsgSendModel model)
        {
            //解析命令
            //将@[?,?]解析
            var mcs = ParseCmd(model.Cmd);
            var t = model.Text;
            foreach (var item in mcs)
            {
                t = t.Replace(item.ParsedText, string.Empty);
            }
            var res = await PutAsync(MsgUrl + "smsg", null, true,
                (nameof(MsgSendModel.Text), t),
                (nameof(MsgSendModel.Mid), model.Mid),
                (nameof(MsgSendModel.Cmd), JsonConvert.SerializeObject(mcs)));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 命令匹配模式
        /// </summary>
        public string MatchePattern  => "@{(.+?)}";
        public List<MsgCmd> ParseCmd(string value)
        {
            var cmds = new List<MsgCmd>();
            var match = Regex.Matches(value, MatchePattern);
            for (int i = 0; i < match.Count; i++)
            {
                var m = match[i];
                var g = m.Groups;
                var rel = g[1].Value;
                var ps = rel.Split(',');
                if (ps.Count() < 2)
                {
                    throw new ArgumentException("参数错误，要求@{id,type,params...}");
                }
                var p = new List<string>(ps.Count() - 2);
                for (int k = 0; k < ps.Count() - 2; k++)
                {
                    p.Add(ps[k + 2]);
                }
                var msgcmd = new MsgCmd(Convert.ToInt32(ps[0]), Convert.ToInt16(ps[1]), p)
                {
                    ParsedText = m.Value,
                    InPos = value.IndexOf(m.Value)
                };
                cmds.Add(msgcmd);
            }
            return cmds;
        }
        #endregion
        #region From_Shop
        /// <summary>
        /// 获取商店列表
        /// <see cref="ShopRepositoryModel"/>
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="autoin">是否自动装载items</param>
        /// <returns></returns>
        public async Task<RelApiReponse<ShopRepositoryModel>> GetShopsAsync(int skip, int take, bool autoin = false)
        {
            var res = await GetAsync(ShopUrl + $"gs/s={skip}/t={take}/ai={autoin}");
            var data = await res.GetForAsync<ShopRepositoryModel>();
            return new RelApiReponse<ShopRepositoryModel>(data,res);
        }
        /// <summary>
        /// 获取商店的物品
        /// <see cref="ShopItemRepositoryModel"/>
        /// </summary>
        /// <param name="sid">商店id</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<ShopItemRepositoryModel>> GetShopItemsAsync(uint sid, int s, int t)
        {
            var res = await GetAsync(ShopUrl + $"gsis/sid={sid}/s={s}/t={t}");
            var data = await res.GetForAsync<ShopItemRepositoryModel>();
            return new RelApiReponse<ShopItemRepositoryModel>(data,res);
        }
        /// <summary>
        /// 获取商店某个物品具体信息
        /// <see cref="ShopItem"/>
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<ShopItem>> GetShopItemDetailAsync(uint sid)
        {
            var res = await GetAsync(ShopUrl + $"gsi/sid={sid}");
            var data = await res.GetForAsync<ShopItem>();
            return new RelApiReponse<ShopItem>(data,res);
        }
        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="iid">物品id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> BuyAsync(uint iid)
        {
            var res = await PutAsync(ShopUrl + "buy", null, true, (nameof(iid), iid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        /// <summary>
        /// 查询用户已经购买的物品
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<ShopItemRepositoryModel>> GetAlreadyBougthItemsAsync(int s,int t)
        {
            var res = await PostAsync(ShopUrl + "uis", true, (nameof(s), s), (nameof(t), t));
            var data = await res.GetForAsync<ShopItemRepositoryModel>();
            return new RelApiReponse<ShopItemRepositoryModel>(data,res);
        }
        /// <summary>
        /// 获取用户持有的物品
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<RelApiReponse<ShopItemRepositoryModel>> GetHasItemAsync(int s,int t)
        {
            var res = await PostAsync(ShopUrl + "gbis", true, (nameof(s), s), (nameof(t), t));
            var data = await res.GetForAsync<ShopItemRepositoryModel>();
            return new RelApiReponse<ShopItemRepositoryModel>(data,res);
        }
        /// <summary>
        /// 使用物品
        /// <see cref="ShopItem"/>
        /// </summary>
        /// <param name="iid">物品id</param>
        /// <returns></returns>
        public async Task<RelApiReponse<bool>> UseItemAsync(int iid)
        {
            var res = await PostAsync(ShopUrl + "uit", true, (nameof(iid), iid));
            var data = await res.GetForAsync<bool>();
            return new RelApiReponse<bool>(data,res);
        }
        #endregion
    }
}
