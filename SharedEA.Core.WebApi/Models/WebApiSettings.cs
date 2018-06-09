using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
{
    public class WebApiSettings
    {
        public WebApiSettings()
        {
        }

        public WebApiSettings(string host, string secretKey)
        {
            Host = host;
            SecretKey = secretKey;
        }

        public string Host { get; set; }

        public string SecretKey { get; set; }
    }
}
