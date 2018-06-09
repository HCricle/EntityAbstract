using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class RelFriend:Friend
    {
        public RelFriend()
        {
        }

        public string TargetName { get; set; }
    }
}
