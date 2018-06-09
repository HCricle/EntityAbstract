using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using JWT;
using JWT.Serializers;
using JWT.Algorithms;
namespace SharedEA.Core.WebApi.Helpers
{
    public static class ConvertHelper
    {
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string key,string result)
        {
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder);
            var json = decoder.Decode(result, Encoding.ASCII.GetBytes(key), verify: true);//token为之前生成的字符串
            return json;
        }
    }
}
