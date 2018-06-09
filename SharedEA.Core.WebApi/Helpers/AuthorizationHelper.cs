using System;
using System.Linq;
using SharedEA.Core.WebApi.JWT;

namespace SharedEA.Core.WebApi.Helpers
{
    public static class AuthorizationHelper
    {
        public static TokenDec ParseToken(string token,string key)
        {
            try
            {
                var tokenDec = ConvertHelper.Base64Decode(key, token);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TokenDec>(tokenDec);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static TokenDec ParseTokenByAuthorization(string authorization,string key)
        {
            if (authorization==null)
            {
                return null;
            }
            var dec = authorization.Split(' ').Last();
            return ParseToken(dec, key);

        }
    }
}
