using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedEA.Core.WebApi.JWT
{
    public class TokenEntity
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
