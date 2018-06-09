using Microsoft.AspNetCore.Mvc;
using SharedEA.Core.WebApi.Helpers;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedEA.Core.Web.Extensions
{
    public static class ControllerExtension
    {
        public static TokenDec GetTokenDec(this Controller controller,string secretKey)
        {
            return AuthorizationHelper.ParseTokenByAuthorization(controller.Request.Headers["Authorization"], secretKey);
        }
        public static string GetAuthorization(this Controller controller)
        {
            return controller.Request.Headers["Authorization"];
        }
    }
}
