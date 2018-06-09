using EntityAbstract.Web.Helpers;
using EntityAbstract.Web.Models.ContentViewModels;
using EntityAbstract.Web.Models.HomeViewModels;
using EntityAbstract.Web.Models.SharedViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedEA.Core.Data.Models;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Controllers
{
    [Route("content")]
    [Authorize]
    public class ContentController : Controller
    {
        private static readonly uint DefaultGroup = 1;
        private readonly EaDbContext _dbContext;
        private readonly UserManager<EaUser> _userManager;
        private readonly SignInManager<EaUser> _signInManager;
        private readonly GroupRepository _groupRepository;
        private readonly ContentRepository _contentRepository;
        private readonly LikeRepository _likeRepository;
        private readonly CommentRepository _commentRepository;
        private readonly ContentFileRepository _contentFileRepository;
        public ContentController(EaDbContext dbContext,
            UserManager<EaUser> userManager,
            ContentRepository contentRepository,
            SignInManager<EaUser> signInManager,
            GroupRepository groupRepository,
            LikeRepository likeRepository,
             CommentRepository commentRepository,
             ContentFileRepository contentFileRepository)
        {
            _userManager = userManager;
            _contentRepository = contentRepository;
            _signInManager = signInManager;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _contentFileRepository = contentFileRepository;
            _groupRepository = groupRepository;
            _dbContext = dbContext;
        }
        [HttpGet("Index/page={page}", Name = "Index")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page, int count = 6)
        {
            var cpm = await _contentRepository.GetContentsAsync(DefaultGroup, page * count, count, false);

            var model = new IndexContentViewModel()
            {
                Contents = cpm.Datas,
                LocPage = page,
                PrevRoutes = new RoutePair[] { new RoutePair("page", page - 1) },
                NextRoutes = new RoutePair[] { new RoutePair("page", page + 1) },
                PrePageCount = count,
                GroupDesp = "所有组内容",
                TotalCount = cpm.Total
            };
            return View(nameof(Index),model);
        }
        [HttpGet("GoMakeContent")]
        [AllowAnonymous]
        public async Task<IActionResult> GoMakeContent(MakeContentViewModel model = null)
        {
            var cpm = await _contentRepository.GetGroupsAsync(0, 999);//所有组--以后会换
            var items = cpm.Datas.Select(g => new SelectListItem() { Value = g.Id.ToString(), Text = g.Name }).ToList();
            if (model == null)
            {
                model = new MakeContentViewModel();
            }
            model.SendGroups = items;
            model.TargetGroup = cpm.Datas.FirstOrDefault()?.Name;
            return View("_MakeContent", model);
        }
        #region NonActions  
        [NonAction]
        public IActionResult Index(IndexContentViewModel model)
        {
            return View(model);
        }
        [NonAction]
        private async Task<IActionResult> GetDetailAsync(uint contentId, int locpage, int prePageCount = 6, bool useJs = false, CommentSendModel sendModel = null)
        {
            var res = await MakeDetailViewModelAsync(contentId, locpage, prePageCount, useJs, sendModel);
            if (res == null)
            {
                return View(ViewHelper.MessageView, new MessageModel("错误内容id", "Home", "Index", "主页"));
            }
            return View("_ContentDetail", res);
        }
        [NonAction]
        public IActionResult CheckSignIn()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return View(ViewHelper.MessageView, new MessageModel() { Controller = "Account", Action = "Login", GoText = "登陆", Text = "请先登陆!" });
            }
            return null;
        }
        [NonAction]
        private IActionResult GoMessageView(string text)
        {
            return View(ViewHelper.MessageView, new MessageModel() { Controller = "Home", Action = "Index", GoText = "主页", Text = text });
        }
        [NonAction]
        private async Task<IActionResult> CheckContent(uint id, string text)
        {
            if (id <= 0 && id >= await _contentRepository.GetCountAsync())
            {
                return GoMessageView("非法内容id");
            }
            return null;
        }
        [NonAction]
        private async Task<ContentDetailViewModel> MakeDetailViewModelAsync(uint contentId, int locpage, int prePageCount = 6, bool useJs = false, CommentSendModel sendModel = null)
        {

            var content = await _contentRepository.GetContentAsync(contentId);
            await _contentRepository.LoadFilesAsync(content);

            if (content == null)
            {
                return null;
            }
            bool like = false;
            var user =await _userManager.GetUserAsync(User);
            if (User != null)
            {
                like = await _contentRepository.IsLikeContentAsync(user, contentId);
            }
            var comms = await _contentRepository.GetContentCommentsAsync(content.Id, prePageCount, locpage * prePageCount);
            foreach (var item in comms.Datas)
            {
                item.UserName = await _dbContext.Users.Where(u => u.Id == item.EaUserId).Select(u => u.UserName).FirstOrDefaultAsync();
            }
            await _contentRepository.LoadFilesAsync(content);
            var retModel = new ContentDetailViewModel(content)
            {
                Sender = user,
                Comments = comms.Datas,
                IsLike = like,
                SendModel = sendModel ?? new CommentSendModel() { ContentId = contentId },
                LocPage = locpage,
                UseJs = useJs,
                TotalCount=comms.Total
            };
            return retModel;
        }
        #endregion
        [HttpGet("cid={cid}")]
        [HttpGet("cid={cid}/lp={lp}/ppc={ppc}/ujs={ujs}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(uint cid, int lp = 0, int ppc = 6, bool ujs = false)
        {
            return await GetDetailAsync(cid, lp, ppc, useJs: ujs, sendModel: null);
        }
        [HttpPost(Name = "MakeContent")]
        public async Task<IActionResult> MakeContent(MakeContentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = CheckSignIn();
                if (check != null)
                {
                    return check;
                }
                var gidValue = model.TargetGroup;
                int gid = -1;
                if (gidValue!=null)
                {
                    gid = Convert.ToInt32(gidValue);
                }
                var res = await _contentRepository.SendContentAsync(_userManager.GetUserId(User),
                    model.Content,
                    model.Title, 
                    model.Lable,
                    gid,
                    model.Files?.Select(f=>new FormFileModel(f)),
                    model.AutoJs);//-1是暂定
                if (res != null)
                {
                    ModelState.AddModelError(string.Empty, res);
                    return View("_MakeContent",model);
                }

                return GoMessageView("发送成功");
            }
            return View("_MakeContent",model);
        }
        [HttpPost("DeleteCommend")]
        [Authorize]
        public async Task<IActionResult> DeleteCommend([FromQuery]uint id, [FromQuery]uint cid)
        {
            var res = await _commentRepository.DeleteCommentAsync(_userManager.GetUserId(User), id);
            if (!res)
            {
                return GoMessageView("删除失败");
            }
            return await Get(cid);
        }
        [HttpGet("path={path}/name={name}",Name ="GetFile")]
        [AllowAnonymous]
        public IActionResult GetFile(string path,string name)
        {
            var res = System.IO.File.Open(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path, name), FileMode.Open);
            var resual = File(res, "application/octet-stream");
            return resual;
        }
        /// <summary>
        /// 这个是删除帖子
        /// </summary>
        /// <param name="id">帖子id</param>
        /// <returns></returns>
        [HttpPost(Name = "DeleteContent")]
        [Authorize]
        public async Task<IActionResult> DeleteContent([FromQuery]uint id, [FromQuery]int page)
        {
            var res = await _contentRepository.DeleteContentAsync(_userManager.GetUserId(User), id);
            if (!res)
            {
                return GoMessageView("删除失败");
            }
            return GoMessageView("删除成功");
        }
        [HttpPost("cid={cid}/lp={lp}/ppc={ppc}/ujs={ujs}",Name = "Like")]
        [Authorize]
        public async Task<IActionResult> Like(uint cid, int lp, int ppc = 6,bool ujs=false)//点赞
        {
            var resModel = await MakeDetailViewModelAsync(cid, lp, ppc, ujs);
            resModel.IsLike = await _likeRepository.LikeForContentAsync(resModel.Sender.Id, cid);
            return RedirectToAction("_ContentDetail", resModel);
        }
        [HttpPost("SendCommend")]
        [Authorize]
        public async Task<IActionResult> SendCommend([FromForm]CommentSendModel model,[FromQuery] uint cid)
        {
            if (ModelState.IsValid)
            {
                //TODO:判断发送的评论是否有script,iframe,object标签
                model.ContentId = cid;
                var res = await _commentRepository.SendCommentAsync(_userManager.GetUserId(User), model.ContentId, model.SendText);
                if (res != null)
                {
                    ModelState.AddModelError(string.Empty, res);
                    return await Get(model.ContentId);
                }
                return await Get(model.ContentId);
            }
            return await GetDetailAsync(model.ContentId,
                locpage: 0,
                sendModel: new CommentSendModel()
                {
                    SendText = model.SendText,
                    ContentId = model.ContentId
                });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("key={key}/page={page}")]
        [HttpGet("page={page}")]
        [HttpGet("SearchContent")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchContent(string key=default(string), int page=0, int prePageCount = 6)
        {
            if (string.IsNullOrEmpty(key))
            {
                if (Request.Query.ContainsKey("ipContent"))
                {
                    key = Request.Query["ipContent"];
                }
            }
            if (string.IsNullOrEmpty(key))
            {
                return await Index(0,6);
            }
            var shareRoute = new RoutePair("key", key);
            var cpm = await _contentRepository.SearchContentAsync(key, page * prePageCount, prePageCount, false);

            var model = new IndexContentViewModel()
            {
                TotalCount = cpm.Total,
                Contents = cpm.Datas,
                Action = nameof(SearchContent),
                PrevRoutes = new RoutePair[] { shareRoute, new RoutePair("page", page - 1) },
                NextRoutes = new RoutePair[] { shareRoute, new RoutePair("page", page + 1) },
                LocPage = page,
                PrePageCount = prePageCount,
                GroupDesp = $"搜索 {key} 的内容",
            };
            return View(nameof(Index), model);
        }
    }
}
