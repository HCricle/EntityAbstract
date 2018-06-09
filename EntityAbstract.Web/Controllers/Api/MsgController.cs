using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.Web.Extensions;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Core.WebApi.Models;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Controllers.Api
{
    /// <summary>
    /// 对消息的控制,
    /// 或者可以改成web端然后apicontroller可以继承使用
    /// </summary>
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class MsgController : Controller
    {
        private readonly MsgRepository _msgRepository;
        public MsgController(MsgRepository msgRepository)
        {
            _msgRepository = msgRepository;
        }
        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("gm")]
        public async Task<IActionResult> GetMsg([FromForm]int s, [FromForm]int t)
        {
            var res = await _msgRepository.GetMsgAsync(this.GetAuthorization(), s, t);
            return Json(res);
        }
        /// <summary>
        /// 获取消息详细
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="isdes">是否倒序</param>
        /// <returns></returns>
        [HttpPost("gmds")]
        public async Task<IActionResult> GetMsgDetails([FromForm] uint mid, [FromForm] int s, [FromForm] int t,[FromForm] bool isdes)
        {
            var res = await _msgRepository.GetMsgDetailsAsync(this.GetAuthorization(), mid, s, t,isdes);
            return Json(res);
        }
        /// <summary>
        /// 发起消息,如果已存在返回true,成功返回false
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpPut("cm")]
        public async Task<IActionResult> CreatMsg([FromForm]string fid)
        {
            var res = await _msgRepository.CreatMsgAsync(this.GetAuthorization(), fid);
            return Json(res);
        }
        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="mid">消息id</param>
        /// <returns></returns>
        [HttpDelete("dm/mid={mid}")]
        public async Task<IActionResult> DeleteMsg( uint mid)
        {
            var res = await _msgRepository.DeleteMsgAsync(this.GetAuthorization(), mid);
            return Json(res);
        }
        /// <summary>
        /// 查询是否需要更新
        /// </summary>
        /// <param name="mid">会话id</param>
        /// <param name="lmid">用户最后一条消息id</param>
        /// <returns></returns>
        [HttpPost("qnu")]
        public async Task<IActionResult> QuireNeedUpdate([FromForm] uint mid,[FromForm] uint lmid)
        {
            var res = await _msgRepository.QuireNeedUpdateAsync(this.GetAuthorization(), mid, lmid);
            return Json(res);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("smsg")]
        public async Task<IActionResult> SendMsg([FromForm] MsgSendModel model)
        {
            var res = await _msgRepository.SendMsgAsync(this.GetAuthorization(), model);
            return Json(res);

        }
    }
}
