using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public class BuyRepository : Repository<Buy, EaDbContext>
    {
        private readonly UserManager<EaUser> _userManager;
        private readonly ShopItemRepository _shopItemRepository;
        public BuyRepository(EaDbContext dbContext,
            UserManager<EaUser> userManager,
            ShopItemRepository shopItemRepository) 
            : base(dbContext,dbContext.Buys)
        {
            _userManager = userManager;
            _shopItemRepository = shopItemRepository;
        }

        public override Task AutoFillAsync(IEnumerable<Buy> entities)
        {
            return Task.CompletedTask;
        }
        public async Task<bool> BuyAsync(EaUser user,uint itemId)
        {
            var ic = await _shopItemRepository.DbSet.SingleOrDefaultAsync(i => i.Id == itemId);
            if (ic == null)
            {
                return false;
            }
            if (user.Money - ic.Price <= 0)
            {
                return false;
            }
            user.Money -= ic.Price;
            await _userManager.UpdateAsync(user);
            DbSet.Add(new Buy()//添加记录
            {
                ShopItemId = itemId,
                EaUserId = user.Id
            });
            return true;
        }
    }
}
