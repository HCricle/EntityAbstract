using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SharedEA.Core.Service.Models;
using SharedEA.Core.Service;

namespace SharedEA.Core.Service.Extensions
{
    public static class EmailSenderExtensions
    {
        public static async Task<IEmailSendedResual> SendEmailConfirmationAsync(this IEmailSenderService emailSender, string email, string link)
        {
            return await emailSender.SendEmailAsync(email, "确认你的邮件",
                $"请点击下面的连接来确认你的邮箱: <a href='{HtmlEncoder.Default.Encode(link)}'>确认</a>");
        }
        /// <summary>
        /// 发送邮件随机验证码
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="email"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static async Task<(IEmailSendedResual,int)> SendEmailRandomCodeAsync(this IEmailSenderService emailSender,string email,int min=100000,int max=999999)
        {
#if !DEBUG
            var code = new Random().Next(min, max);
            return (await emailSender.SendEmailAsync(email, "你的邮件验证码", $"你的账号邮件验证码为:{code},请注意保密哦！"), code);
#else
            var res = new EmailSendedResual(HttpStatusCode.OK, null, null);
            return await Task.FromResult((res, 197238));
#endif
        }
    }
    
}
