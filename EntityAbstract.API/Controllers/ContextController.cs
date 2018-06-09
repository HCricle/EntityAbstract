using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedEA.Core.Data.Models;
using SharedEA.Core.Model.DbContext;
using SharedEA.Core.Model.DbModels;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Data.Repositories;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EntityAbstract.Api.Controllers
{
    /// <summary>
    /// 这里是内存操作的api控制器,没完成，而且account也没完成
    /// 
    /// 暂时只读
    /// </summary>
    [Produces("application/json")]//application/x-www-form-urlencoded
    [Consumes("application/json", "multipart/form-data")]
    [Route("api/v1/[controller]")]
    public class ContextController : Controller
    {
        private readonly EaDbContext _dbContext;
        private readonly IOptions<WebApiSettings> _settings;
        private readonly SignInManager<EaUser> _signInManager;
        private readonly GroupRepository _groupRepository;
        private readonly ContentRepository _contentRepository;
        private readonly CommentRepository _commentRepository;
        private readonly LikeRepository _likeRepository;
        public ContextController(EaDbContext dbContext,
            IOptions<WebApiSettings> settings,
            SignInManager<EaUser> signInManager,
            ContentRepository contentRepository,
            GroupRepository groupRepository,
            CommentRepository commentRepository,
            LikeRepository likeRepository)
        {
            _dbContext = dbContext;
            _settings = settings;
            _signInManager = signInManager;
            _contentRepository = contentRepository;
            _groupRepository = groupRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
        }

        [HttpGet("groups")]
        [AllowAnonymous]
        public async Task<IActionResult> Groups()
        {
            return Json(await _groupRepository.GetAllAsync());
        }
        [HttpGet("Groups/name={name}")]
        [AllowAnonymous]
        public async Task<IActionResult> Groups([FromForm]string name)
        {
            return Json(await _groupRepository.GetAsync(g => g.Name == name));
        }
        //获取内容
        [HttpGet("Contents")]
        [AllowAnonymous]
        public async Task<IActionResult> Contents([FromForm]int gid, [FromForm]int page, [FromForm]bool autoin = false)//一页6个
        {
            return Json(await _contentRepository.GetAsync(c=>c.Id== gid, 6, page * 6, autoin));
        }
        [HttpPut("SendContent")]
        //[AllowAnonymous]
        public async Task<IActionResult> SendContent(IFormCollection files)
        {
            /*
            var fss = files.Files.Select(f => new { f.Name, f.Length });
            var dic = new Dictionary<string, string[]>();
            for (int i = 0; i < files.Keys.Count; i++)
            {
                dic.Add(files.Keys.ElementAt(i), files[files.Keys.ElementAt(i)]);
                Console.WriteLine($"{files.Keys.ElementAt(i)}, {files[files.Keys.ElementAt(i)].FirstOrDefault()}");
            }
            //var title = dic["title"].FirstOrDefault();
            return await Task.FromResult(Json(new {dic, dic.Count, fss }));
            */

            if (files.Any(f=>f.Value.Count==0))
            {
                return BadRequest("模型验证失败");
            }
            //var uid = files["Uid"].FirstOrDefault();
            var tokendec = GetTokenDec();
            if (tokendec==null)
            {
                return BadRequest("token 验证失败");
            }
            var content = files["Content"].FirstOrDefault();
            var title = files["Title"].FirstOrDefault();
            var label = files["Label"].FirstOrDefault();
            //创建keypairs
            var res = await _contentRepository.SendContentAsync(await FindIdByName(tokendec.sub), content, title, label, -1, files.Files.Select(f => new FormFileModel(f)));
            if (res != null)
            {
                return BadRequest(res);
            }
            return Ok();
        }
        [HttpGet("Contents/gid={gid}/page={page}/userid={userid}/autoin={autoin}")]
        [AllowAnonymous]
        public async Task<IActionResult> Contents(int gid, int page,string userid, bool autoin=false)//一页6个
        {
            return Json(await _contentRepository.GetAsync((c => c.ContentGroupId == gid && c.EaUserId == userid && c.IsEnable), 6, page * 6, autoin));
        }
        [HttpGet("SearchContent/ts={ts}/autoin={autoin}")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchContent(string ts, int page,bool autoin=false, int preCount = 6)
        {
            return Json((await _contentRepository.GetAsync(c => c.Title.Contains(ts), preCount, preCount * page, autoin)));
        }
        [HttpGet("GroupCount")]
        [AllowAnonymous]
        public async Task<IActionResult> GroupCount()
        {
            return Json(await _groupRepository.GetCountAsync());
        }

        [HttpGet("ContentCount")]
        [AllowAnonymous]
        public async Task<IActionResult> ContentCount()
        {
            return Json(await _contentRepository.GetCountAsync());
        }
        //获取某用户发的内容
        [HttpGet("UserContent/page={page}")]
        public async Task<IActionResult> UserContent(int page, int preCount = 6)
        {
            var token = GetTokenDec();
            if (token==null)
            {
                return BadRequest("token 验证失败");
            }
            var id = await FindIdByName(token.sub);
            return Json((await _contentRepository.GetAsync(c => c.EaUserId == id && c.IsEnable, preCount, preCount * page)));
        }
        /// <summary>
        /// 获取内容的评论
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="page"></param>
        /// <param name="preCount"></param>
        /// <returns></returns>
        [HttpGet("ContentComments/cid={cid}/page={page}")]
        [AllowAnonymous]
        public async Task<IActionResult> ContentComments(uint cid,int page,int preCount=6)
        {
            return Json((await _commentRepository.GetAsync(c => c.ContentId == cid && c.IsEnable, preCount, preCount * page)));
        }
        // PUT api/v1/context/SendComment
        [HttpPut("SendComment")]
        public async Task<IActionResult> SendComment([FromForm] uint cid,[FromForm] string cstring)
        {
            var content = (await _contentRepository.GetAsync(c => c.Id == cid)).FirstOrDefault();
            if (content==null)
            {
                return BadRequest("内容id错误");
            }
            if (string.IsNullOrEmpty(cstring))
            {
                return BadRequest("内容为空");
            }
            var tokenDec = GetTokenDec();
            if (tokenDec==null)
            {
                return BadRequest("token 验证失败");
            }
            _dbContext.Comments.Add(new Comment()
            {
                ContentId = content.Id,
                CreateTime = DateTime.Now,
                HtmlContent = cstring,
                EaUserId = await FindIdByName(tokenDec.sub)
            });
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete("DeleteComment/cid={cid}")]
        public async Task<IActionResult> DeleteComment(uint cid)
        {
            var token = GetTokenDec();
            if (token==null)
            {
                return BadRequest("错误 token");
            }
            var uid = await FindIdByName(token.sub);
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest("错误 uid");
            }
            var comment = (await _commentRepository.GetAsync(c => c.Id == cid&&c.EaUserId==uid)).FirstOrDefault();
            if (comment==null)
            {
                return BadRequest("无法删除");
            }
            _dbContext.Comments.Remove(comment);
            _dbContext.SaveChanges();   
            return Ok();
        }
        [HttpDelete("DeleteContent/cid={cid}")]
        public async Task <IActionResult> DeleteContent(uint cid)
        {
            var token = GetTokenDec();
            if (token == null)
            {
                return BadRequest("错误 token");
            }
            var uid = await FindIdByName(token.sub);
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest("错误 uid");
            }
            var content = (await _contentRepository.GetAsync(c => c.Id == cid && c.EaUserId == uid)).FirstOrDefault();
            if (content == null)
            {
                return BadRequest("无法删除");
            }
            _dbContext.Contents.Remove(content);
            _dbContext.SaveChanges();
            return Ok();
        }
        [NonAction]
        public TokenDec GetTokenDec()
        {
            var authori = Request.Headers["Authorization"];
            if (authori.Count() > 0)
            {
                var dec = authori.First().Split(" ").Last();
                return Helpers.AuthorizationHelper.ParseToken(dec, _settings.Value.SecretKey);
            }
            return null;
        }
        [NonAction]
        public async Task<string> FindIdByName(string name)
        {
            var user =await _signInManager.UserManager.FindByNameAsync(name);
            return user.Id;
        }
    }
}
