using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class FriendRepositoryModel : ArrayRepository<Friend>
    {
        public FriendRepositoryModel()
        {
        }

        public FriendRepositoryModel(int totalCount, int skip, int take, params Friend[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
