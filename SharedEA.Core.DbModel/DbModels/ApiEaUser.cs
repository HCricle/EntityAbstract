using System;
using System.Collections.Generic;
using System.Text;
namespace SharedEA.Core.DbModel.DbModels.Api
{
    /// <summary>
    /// 用户
    /// </summary>
    public class EaUser
    {
        public string Id { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }
        public string NormalizedEmail { get; set; }
        public string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 头像目录部分~/wwwroot/{HeadImgPath}
        /// </summary>
        public static readonly string HeadImgPath = "heads";
        /// <summary>
        /// 用户类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 头像路径 ~/wwwroot/heads/{HImg}
        /// </summary>
        public string HImg { get; set; }
        /// <summary>
        /// 等级，现在不启用
        /// </summary>
        public ushort Level { get; set; }
        /// <summary>
        /// 钱
        /// </summary>
        public int Money { get; set; }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
