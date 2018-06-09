using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EntityAbstract.Web.Models;
using EntityAbstract.Web.Models.HomeViewModels;
using Microsoft.AspNetCore.Identity;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EntityAbstract.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly EaDbContext _dbContext;
        private readonly UserManager<EaUser> _userManager;
        private readonly SignInManager<EaUser> _signInManager;
        private readonly ContentRepository _contentRepository;
        private readonly ContentFileRepository _contentFileRepository;
        public HomeController(EaDbContext dbContext,UserManager<EaUser> userManager ,SignInManager<EaUser> signInManager,
            ContentRepository contentRepository,
            ContentFileRepository contentFileRepository)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _contentRepository = contentRepository;
            _contentFileRepository = contentFileRepository;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(new IndexViewModel() { ImgCount=await GetImgCountAsync()});
        }
        [HttpGet(Name =nameof(RandomIndex))]
        public async Task<IActionResult> RandomIndex()
        {
            var model = new IndexViewModel();
            var img = await RandGetImgAsync();
            if (img.Item1 != null)
            {
                model.Imgs.Add(img.Item1);
            }
            model.ImgCount = img.Item2;
            return View(nameof(Index), model);
        }
        [NonAction]
        private async Task<(ContentFile,int)> RandGetImgAsync()
        {
            var imgCount = await GetImgCountAsync();
            if (imgCount > 0)//随机找一个图给你
            {
                var imgs = _contentFileRepository.DbSet.AsNoTracking()
                    .Where(f => f.UseType == "img").Select(i =>  i.Id).ToArray();
                var selectImg = imgs[new Random().Next(0, imgs.Count() )];
                return (_contentFileRepository.DbSet.AsNoTracking().Where(f => f.Id == selectImg).FirstOrDefault(),imgCount);
            }
            return (null,imgCount);
        }
        [NonAction]
        private async Task<int> GetImgCountAsync()
        {
            return await _contentFileRepository.DbSet.AsNoTracking().Where(f => f.UseType == "img").CountAsync();
        }
        [HttpGet("PcClient")]
        public IActionResult PcClient()
        {
            var res = System.IO.File.Open(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "clients", "pc","Ea v1.0.zip"), FileMode.Open);
            var resual = File(res, "application/octet-stream","pc-Ea v1.0.zip");
            return resual;
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Studing at 2018";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
