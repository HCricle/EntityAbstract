using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class RelComment:Comment
    {
        public string UserName { get; set; }
        public bool IsSelf { get; set; }
    }
}
