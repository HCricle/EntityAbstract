using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Shared.Core.Api.Extensions
{
    public static class MultipartFormDataContentExtensions
    {
        public static void AddFile(this MultipartFormDataContent content, string name, Stream stream)
        {
            var sc = new StreamContent(stream);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            // "application/octet-stream"
            content.Add(sc, name);//key
        }
    }
}
