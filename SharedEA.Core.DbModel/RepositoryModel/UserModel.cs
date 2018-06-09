using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
    }
}
