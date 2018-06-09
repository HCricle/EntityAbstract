using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Core.Web.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void AddAuthorizeFilter(this MvcOptions options)
        {
            var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }
    }
}
