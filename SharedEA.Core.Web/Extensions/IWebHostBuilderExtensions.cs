using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
namespace SharedEA.Core.Web.Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseResponseCompression(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(server =>
            {
                server.AddResponseCompression();
            })
            .Configure(app =>
            {
                app.UseResponseCompression();
            });
            return builder;
        }
    }
}
