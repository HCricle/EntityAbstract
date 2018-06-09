using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Api.Models
{
    /// <summary>
    /// 注册的数据模型
    /// </summary>
    public class RegisterModel
    {
        public RegisterModel()
        {
        }

        public RegisterModel(string name, string email, string pwd)
        {
            Name = name;
            Email = email;
            Pwd = pwd;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
