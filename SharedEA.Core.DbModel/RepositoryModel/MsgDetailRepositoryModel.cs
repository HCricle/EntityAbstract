using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class MsgDetailRepositoryModel : ArrayRepository<MsgDetail>
    {
        public MsgDetailRepositoryModel()
        {
        }
        public MsgDetailRepositoryModel(bool ok)
        {
            Ok = ok;
        }
        public MsgDetailRepositoryModel(int totalCount, int skip, int take, params MsgDetail[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
