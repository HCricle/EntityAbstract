﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedEA.Core.WebApi.JWT
{
    public class TokenProviderOptions
    {
        public TokenProviderOptions()
        {
        }

        /// <summary>
        /// iss - Issuer 代表这个jwt的签发主体
        /// </summary>
        /// <remarks>(即服务端--本程序)</remarks>
        public string Issuer { get; set; }
        /// <summary>
        /// sub - Subject 代表这个jwt的主体,即它的所有人
        /// </summary>
        /// <remarks>表示  --> 发起请求的用户, 直接看控制器中返回的数据</remarks>
        public string Subject { get; set; }
        /// <summary>
        /// aud - Audience 代表这个jwt的接收对象
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// nbf - Not Before 时间戳,代表这个jwt的生效时间, 即在此之前是无效的
        /// </summary>
        public DateTime NotBefore => DateTime.UtcNow;
        /// <summary>
        /// iat - Issue At 时间戳, 代表这个jwt的签发时间
        /// </summary>
        public DateTime IssueAt => DateTime.Now;
        /// <summary>
        /// 有效时间 : 默认5分钟
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// exp - Expiration 时间戳,代表这个jwt的过期时间
        /// </summary>
        public DateTime Expiration => IssueAt.Add(ValidFor);
        /// <summary>
        /// Func委托方法(对象), 生成 jti - JWT ID 唯一标识
        /// </summary>
        public string JtiGenerator => Guid.NewGuid().ToString();
        /// <summary>
        /// 生成token时使用的签名
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
