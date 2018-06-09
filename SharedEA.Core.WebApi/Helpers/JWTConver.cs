using JWT;
using JWT.Serializers;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.Helpers
{
    public class JWTConver
    {
       /// <summary>
       /// 解析web api传入时，header的jwt加密数据
       /// </summary>
       /// <param name="key"></param>
       /// <param name="result"></param>
       /// <returns></returns>
        public static string DecodeJWT(string key, string result)
        {
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder);
            var json = decoder.Decode(result, Encoding.ASCII.GetBytes(key), verify: true);//token为之前生成的字符串
            return json;
        }
        public static TokenDec ParseToken(string token, string key)
        {
            try
            {
                var tokenDec = DecodeJWT(key, token);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TokenDec>(tokenDec);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
