using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Shared.Core.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddAuthorizationToken(this HttpClient httpClient,string token)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }
    }
}
