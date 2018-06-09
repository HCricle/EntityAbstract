using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Api.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
        }

        public LoginModel(string name, string pwd)
        {
            Name = name;
            Pwd = pwd;
        }

        public string Name { get; set; }
        public string Pwd { get; set; }
    }
}
