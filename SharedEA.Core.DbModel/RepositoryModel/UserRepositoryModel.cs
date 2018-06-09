using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class UserRepositoryModel : ArrayRepository<RelFriend>
    {
        public UserRepositoryModel()
        {
        }

        public UserRepositoryModel(int totalCount, int skip, int take, params RelFriend[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
