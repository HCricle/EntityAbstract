using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.Web.Extensions;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Controllers.Api
{
    /// <summary>
    /// 对朋友的控制
    /// </summary>
    [Route("api/v1/[controller]")]
    [AllowAnonymous]//暂时没办法
    public class FriendController:Controller
    {
        private readonly FriendRepository _friendRepository;
        public FriendController(FriendRepository friendRepository) 
        {
            _friendRepository = friendRepository;//理想情况下
        }
        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="sn">名字包含字符</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpGet("su/sn={sn}/s={s}/t={t}")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchUser(string sn, int s, int t)
        {
            var res = await _friendRepository.SearchFirendAsync(sn, s, t);
            return Json(res);
        }//TODO:下面的

        /// <summary>
        /// 获取朋友
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("gf")]
        public async Task<IActionResult> GetFriends([FromForm]int s, [FromForm]int t)
        {
           var fpm=await _friendRepository.GetFriendsAsync(this.GetAuthorization(), s, t);
            return Json(fpm);
        }
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="fuid">friend uid</param>
        /// <returns></returns>
        [HttpDelete("df/fuid={fuid}")]
        public async Task<IActionResult> DeleteFirend(string fuid)
        {
            var res = await _friendRepository.DeleteFirendAsync(this.GetAuthorization(), fuid);
            return Json(res);
        }
        /// <summary>
        /// 同意朋友的申请
        /// </summary>
        /// <param name="fid">friend id</param>
        /// <returns></returns>
        [HttpPost("afa")]
        public async Task<IActionResult> AcceptFriendApply([FromForm] uint fid)
        {
            var res =await _friendRepository.AcceptFriendApplyAsync(this.GetAuthorization(), fid);
            return Json(res);
        }
        /// <summary>
        /// 获取朋友的申请
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("gafs")]
        public async Task<IActionResult> GetApplyFriends([FromForm]int s,[FromForm]int t)
        {
            var res = await _friendRepository.GetApplyFriendsAsync(this.GetAuthorization(), s, t);
            return Json(res);
        }
        /// <summary>
        /// 提交好友申请,不给你理由去申请o_o
        /// </summary>
        /// <param name="uid">想申请的好友id</param>
        /// <returns></returns>
        [HttpPut("af")]
        public async Task<IActionResult> ApplyFriend([FromForm]string uid)
        {
            var res = await _friendRepository.ApplyFriendAsync(this.GetAuthorization(), uid);
            return Json(res);
        }
    }
}
