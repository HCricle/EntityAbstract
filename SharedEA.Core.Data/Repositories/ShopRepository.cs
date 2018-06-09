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
    public class ShopRepository : Repository<Shop, EaDbContext>
    {
        private readonly ShopItemRepository _shopItemRepository;
        private readonly BuyRepository _buyRepository;
        private readonly IOptions<WebApiSettings> _settings;
        private readonly UserManager<EaUser> _userManager;
        public ShopRepository(EaDbContext dbContext,
            IOptions<WebApiSettings> settings,
            UserManager<EaUser> userManager,
            ShopRepository shopRepository,
            ShopItemRepository shopItemRepository,
            BuyRepository buyRepository) 
            : base(dbContext,dbContext.Shops)
        {
            _settings = settings;
            _userManager = userManager;
            _buyRepository = buyRepository;
            _shopItemRepository = shopItemRepository;

        }
        public async override Task AutoFillAsync(IEnumerable<Shop> entities)
        {
            foreach (var item in entities)
            {
                item.Items = await _shopItemRepository.Where(s => s.ShopId == item.Id).ToList();//装载item
            }
        }
        public async Task<ShopItemRepositoryModel> GetUserItemsAsync(EaUser user, int s, int t,bool searchUnEnable=false)
        {
            var sis = _buyRepository.Where(b => (!searchUnEnable&&b.IsEnable) && b.EaUserId == user.Id)
                                     .Skip(s)
                                     .Take(t);
            var count = await sis.Count();
            var sisid = await sis.Select(d => d.ShopItemId).ToArray();//已购买的商品id
            var datas = await _shopItemRepository.Where(si => sisid.Contains(si.Id)).ToArray();
            var model = new ShopItemRepositoryModel(count, s, t, datas);
            return model;
        }
        /// <summary>
        /// 获取用户已购买的商品
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<ShopItemRepositoryModel> GetUserItemsAsync(string authorization,int s, int t)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new ShopItemRepositoryModel(false);
            }
            return await GetUserItemsAsync(user, s, t,true);
        }
        /// <summary>
        /// 获取用户持有的物品
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<ShopItemRepositoryModel> GetUserHasItemsAsync(string authorization, int s, int t)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new ShopItemRepositoryModel(false);
            }
            return await GetUserItemsAsync(user, s, t);
        }
        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="authrization"></param>
        /// <param name="iid"></param>
        /// <returns></returns>
        public async Task<ShopItem> UseItem(string authorization, uint iid)
        {
            //NEEDFill:需要以后更新
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return null;
            }
            var si = await _buyRepository.SingleOrDefault(b => b.IsEnable && b.Id == iid && b.EaUserId == user.Id);
            if (si==null)
            {
                return null;
            }
            var item = await _shopItemRepository.Single(s => s.Id == si.ShopItemId);
            return item;
        }
        /// <summary>
        /// 获取商店
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="autoin">是否自动装载</param>
        /// <returns></returns>
        public async Task<ShopRepositoryModel> GetShopsAsync(int skip,int take,bool autoin=false)
        {
            var sns = NonTrackingAsyncEnum.Where(s=>s.IsEnable);
            var count = await sns.Count();
            var datas = await sns.Skip(skip).Take(take).ToArray();
            if (autoin)
            {
                await AutoFillAsync(datas);
                foreach (var item in datas)
                {
                    await _shopItemRepository.AutoFillAsync(item.Items);
                }
            }
            return new ShopRepositoryModel(count, skip, take, datas);
        }
        /// <summary>
        /// 获取商店的物品
        /// </summary>
        /// <param name="sid">商店id</param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<ShopItemRepositoryModel> GetShopItemsAsync(uint sid, int s, int t)
        {
            var sis = _shopItemRepository.NonTrackingAsyncEnum.Where(si => si.ShopId == sid && si.IsEnable);
            var count = await sis.Count();
            var datas = await sis.Skip(s).Take(t).ToArray();
            return new ShopItemRepositoryModel(count, s, t, datas);
        }
        /// <summary>
        /// 获取商品的具体信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<ShopItem> GetShopItemAsync(uint sid)
        {
            var si = await _shopItemRepository.NonTrackingAsyncEnum.SingleOrDefault(s => s.Id == sid && s.IsEnable);
            return si;
        }
        public async Task<bool> BuyAsync(EaUser user, uint iid)
        {
            var buyres = await _buyRepository.BuyAsync(user, iid);
            return buyres;
        }
        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="iid"></param>
        /// <returns></returns>
        public async Task<bool> BuyAsync(string authorization, uint iid)
        {
            var user = await GetUserAsync(authorization);
            if (user==null)
            {
                return false;
            }
            var buyres = await _buyRepository.BuyAsync(user, iid);
            return buyres;
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
