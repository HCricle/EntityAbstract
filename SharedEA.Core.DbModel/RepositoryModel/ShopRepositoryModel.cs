using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class ShopRepositoryModel : ArrayRepository<Shop>
    {
        public ShopRepositoryModel()
        {
        }

        public ShopRepositoryModel(bool ok) : base(ok)
        {
        }

        public ShopRepositoryModel(int totalCount, int skip, int take, Shop[] datas, bool ok = true) : base(totalCount, skip, take, datas, ok)
        {
        }
    }
}
