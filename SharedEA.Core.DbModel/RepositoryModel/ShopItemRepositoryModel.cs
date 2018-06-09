using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class ShopItemRepositoryModel : ArrayRepository<ShopItem>
    {
        public ShopItemRepositoryModel(bool ok) : base(ok)
        {
        }

        public ShopItemRepositoryModel(int totalCount, int skip, int take, params ShopItem[] ts) 
            : base(totalCount, skip, take, ts)
        {
        }
    }
}
