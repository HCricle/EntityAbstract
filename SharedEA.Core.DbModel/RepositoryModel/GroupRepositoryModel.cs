using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class GroupRepositoryModel : ArrayRepository<ContentGroup>
    {
        public GroupRepositoryModel(int totalCount, int skip, int take, params ContentGroup[] ts) : base(totalCount, skip, take, ts)
        {
        }

        public GroupRepositoryModel()
        {
        }
        
    }
}
