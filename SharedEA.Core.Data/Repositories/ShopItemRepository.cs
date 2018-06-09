using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public class ShopItemRepository : Repository<ShopItem, EaDbContext>
    {
        private readonly UserManager<EaUser> _userManager;
        private readonly BuyRepository _buyRepository;
        public ShopItemRepository(EaDbContext dbContext, 
            UserManager<EaUser> userManager,
            BuyRepository buyRepository)
            : base(dbContext, dbContext.ShopItems)
        {
            _userManager = userManager;
            _buyRepository = buyRepository;
        }

        public override Task AutoFillAsync(IEnumerable<ShopItem> entities)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="aud"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<bool> UseItemAsync(string aud,uint itemId)
        {
            var user = await _userManager.FindByNameAsync(aud);
            var buy = await _buyRepository.DbSet.SingleOrDefaultAsync(b => b.Id == itemId && b.EaUserId == user.Id && b.IsEnable);
            if (buy==null)//没有购买
            {
                return false;
            }
            buy.IsEnable = false;//已使用
            _buyRepository.DbSet.Update(buy);
            return true;
        }
    }
}
