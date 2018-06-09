using Shared.Core.Api.Models;
using Shared.Core.Api.WebApi;
using System;
using System.Threading.Tasks;

namespace ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var wa = new WebApiManager();
            Task.Run(async () =>
            {
                var l = await wa.LoginAsync(new LoginModel("HCricle", "Asdfg123456"));
                var c = await wa.GetContentsAsync(0, 0);
                var q = await wa.SearchContentAsync("ww", 1,0);
            }).Wait();
        }
    }
}
