using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EntityAbstract.Web.Models.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedEA.Core.Data.Models;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.Web.Extensions;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Data.Repositories;

namespace EntityAbstract.Api.Controllers
{
    /// <summary>
    /// 这里是内存操作的api控制器,没完成，而且account也没完成
    /// 
    /// 暂时只读
    /// </summary>
    [Produces("application/json")]//application/x-www-form-urlencoded
    [Consumes("application/json", "multipart/form-data", "application/x-www-form-urlencoded")]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class ContentController : Controller
    {
        private readonly ContentRepository _contentRepository;
        public ContentController(ContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }
        [HttpGet("g/s={s}/t={t}")]
        [AllowAnonymous]
        public async Task<IActionResult> Groups(int s, int t)
        {
            var res = await _contentRepository.GetGroupsAsync(s, t);
            return Json(res);
        }
        [HttpGet("gbi/gid={gid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Groups(uint gid)
        {
            var res = await _contentRepository.GetGroupAsync(gid);
            return Json(res);
        }
        //获取内容
        [HttpGet("c/gid={gid}/s={s}/t={t}/ai={ai}")]
        [AllowAnonymous]
        public async Task<IActionResult> Contents(uint gid, int s = 0, int t = 6, bool ai = false)//一页6个
        {
            var res = await _contentRepository.GetContentsAsync(gid, s, t, ai);
            return Json(res);
        }
        [HttpPost("ilc")]
        public async Task<IActionResult> IsLikeContent([FromForm]uint cid)
        {
            var res = await _contentRepository.IsLikeContentAsync(this.GetAuthorization(), cid);
            return Json(res);
        }
        [HttpPut("lc/cid={cid}")]
        public async Task<IActionResult> LikeContent(uint cid)
        {
            var r = await _contentRepository.LikeContentAsync(this.GetAuthorization(), cid);
            return Json(r);
        }
        // 获取用户喜欢的内容
        [HttpPost("ulc")]
        public async Task<IActionResult> UserLikeContents([FromForm]int t, [FromForm]int s)
        {
            var r = await _contentRepository.GetLikeContentAsync(this.GetAuthorization(), t, s);
            return Json(r);
        }
        [HttpGet("sct/cid={cid}")]
        [AllowAnonymous]
        public async Task<IActionResult> SingleContent(uint cid)
        {
            var c = await _contentRepository.GetGroupAsync(cid);
            return Json(c);
        }
        [HttpDelete("ulc/cid={cid}")]
        public async Task<IActionResult> UnLikeContent(uint cid)
        {
            var r = await _contentRepository.UnLikeContentAsync(this.GetAuthorization(), cid);
            return Json(r);
        }


        [HttpPut("sc")]
        //[AllowAnonymous]
        public async Task<IActionResult> SendContent(IFormCollection files)
        {
            var res = await _contentRepository.SendContentAsync(this.GetAuthorization(), files);
            return Json(res);
        }
        [HttpGet("cd/cid={cid}")]
        [AllowAnonymous]
        public async Task<IActionResult> ContentDetail(int cid)
        {
            var c = await _contentRepository.SingleOrDefault(co => co.Id == cid&&co.IsEnable);
            if (c==null)
            {
                return BadRequest("不存在此内容");
            }
            await _contentRepository.LoadFilesAsync(c);
            var model = new ContentDetailViewModel(c);
            return View("~/Views/Shared/_OnlyContent.cshtml", model);
        }
        [HttpGet("sc/ts={ts}/s={s}/t={t}/ai={ai}")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchContent(string ts, int s,int t,bool ai=false)
        {
            var res = await _contentRepository.SearchContentAsync(ts, s, t, ai);
            return Json(res);
        }
        [HttpGet("gc")]
        [AllowAnonymous]
        public async Task<IActionResult> GroupCount()
        {
            return Json(await _contentRepository.GetGroupCountAsync());
        }

        [HttpGet("cc/gid={gid}")]
        [AllowAnonymous]
        public async Task<IActionResult> ContentCount(uint gid)
        {
            return Json(await _contentRepository.GetGroupContentCountAsync(gid));
        }
        //获取某用户发的内容
        [HttpGet("ucs/s={s}/t={t}")]
        public async Task<IActionResult> UserContent(int s,int t)
        {
            var res = await _contentRepository.GetUserContentsAsync(this.GetAuthorization(), s, t);
            return Json(res);
        }
        /// <summary>
        /// 获取内容的评论
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="page"></param>
        /// <param name="preCount"></param>
        /// <returns></returns>
        [HttpGet("ccom/cid={cid}/t={t}/s={s}")]
        [AllowAnonymous]
        public async Task<IActionResult> ContentComments(uint cid,int t,int s)
        {
            var res = await _contentRepository.GetContentCommentsAsync(cid, t, s);
            return Json(res);
        }
        // PUT api/v1/context/SendComment
        [HttpPut("scom")]
        public async Task<IActionResult> SendComment([FromForm] uint cid,[FromForm] string cstring)
        {
            var res = await _contentRepository.SendCommentAsync(this.GetAuthorization(), cid, cstring);
            return Json(res);
        }

        [HttpDelete("dcom/cid={cid}")]
        public async Task<IActionResult> DeleteComment(uint cid)
        {
            var res = await _contentRepository.DeleteCommentAsync(this.GetAuthorization(), cid);
            return Json(res);
        }
        [HttpDelete("dc/cid={cid}")]
        public async Task <IActionResult> DeleteContent(uint cid)
        {
            var res = await _contentRepository.DeleteContentAsync(this.GetAuthorization(), cid);
            return Json(res);
        }
        // 从fid获取文件
        [HttpGet("gf/fid={fid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiles(uint fid)
        {
            var res = await _contentRepository.GetFileAsync(fid);
            if (res.Ok)
            {
                return File(res.Path, res.ContentType, res.File.OriginalName);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
