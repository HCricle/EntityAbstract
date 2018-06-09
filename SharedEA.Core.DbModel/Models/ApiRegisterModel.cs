using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
{
    public class ApiRegisterModel
    {
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string UserName { get; set; }
    }
}
