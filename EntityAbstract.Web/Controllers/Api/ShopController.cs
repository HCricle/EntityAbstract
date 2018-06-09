using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    /// 商店的控制器
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ShopController : Controller
    {
        private readonly ShopRepository _shopRepository;
        private readonly ShopItemRepository _shopItemRepository;
        private readonly BuyRepository _buyRepository;
        private readonly IOptions<WebApiSettings> _settings;
        private readonly UserManager<EaUser> _userManager;
        public ShopController(IOptions<WebApiSettings> settings,
            UserManager<EaUser> userManager,
            ShopRepository shopRepository,
            ShopItemRepository shopItemRepository,
            BuyRepository buyRepository)
        {
            _settings = settings;
            _userManager = userManager;
            _shopRepository = shopRepository;
            _buyRepository = buyRepository;
            _shopItemRepository = shopItemRepository;
        }
        /// <summary>
        /// 获取商店
        /// </summary>
        /// <param name="s">跳过多少个</param>
        /// <param name="t">获取多少个</param>
        /// <param name="ai">是否填充item</param>
        /// <returns></returns>
        [HttpGet("gs/s={s}/t={t}/ai={ai}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShops(int s, int t, bool ai = false)
        {
            var ss = await _shopRepository.GetShopsAsync(s, t,ai);
            return Json(ss);
        }
        /// <summary>
        /// 获取商店的物品
        /// </summary>
        /// <param name="sid">商店id</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpGet("gsis/sid={sid}/s={s}/t={t}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShopItems(uint sid,int s,int t)
        {
            var res = await _shopRepository.GetShopItemsAsync(sid, s, t);
            return Json(res);
        }
        /// <summary>
        /// 获取商品的具体信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        [HttpGet("gsi/sid={sid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShopItem(uint sid)
        {
            var res = await _shopRepository.GetShopItemAsync(sid);
            return Json(res);
        }
        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="iid"></param>
        /// <returns></returns>
        [HttpPut("buy")]
        public async Task<IActionResult> Buy([FromForm]uint iid)
        {
            var res = await _shopRepository.BuyAsync(this.GetAuthorization(), iid);
            return Json(res);
        }
        /// <summary>
        /// 获取用户已购买的商品，包含已经使用的
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("uis")]
        public async Task<IActionResult> GetUserItems([FromForm]int s, [FromForm]int t)
        {
            var res = await _shopRepository.GetUserItemsAsync(this.GetAuthorization(), s, t);
            return Json(res);
        }
        /// <summary>
        /// 获取用户持有的物品
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost("gbis")]
        public async Task<IActionResult> GetUserHasItem([FromForm]int s, [FromForm]int t)
        {
            var res = await _shopRepository.GetUserHasItemsAsync(this.GetAuthorization(), s, t);
            return Json(res);
        }
        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="iid"></param>
        /// <returns></returns>
        [HttpPost("uit")]
        public async Task<IActionResult> UseItem([FromForm] uint iid)
        {
            var res = await _shopRepository.UseItem(this.GetAuthorization(), iid);
            return Json(res);
        }
        //TODO
        [NonAction]
        public TokenDec GetToken()
        {
            return this.GetTokenDec(_settings.Value.SecretKey);
        }
    }
}
