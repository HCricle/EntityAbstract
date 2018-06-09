using System;
using System.Collections.Generic;
using System.Text;

namespace EntityAbstract.Api.Model.Models
{
    public class TokenDec
    {
        /// <summary>
        /// 
        /// </summary>
        public string sub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string jti { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int iat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int nbf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int exp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iss { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string aud { get; set; }
    }
}
