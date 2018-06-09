using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.Data.Repositories
{
    public class RelFriend:Friend
    {
        public string TargetName { get; set; }
    }
}
