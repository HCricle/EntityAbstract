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
            return await emailSender.SendEmailAsync(email, "ȷ������ʼ�",
                $"���������������ȷ���������: <a href='{HtmlEncoder.Default.Encode(link)}'>ȷ��</a>");
        }
        /// <summary>
        /// �����ʼ������֤��
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
            return (await emailSender.SendEmailAsync(email, "����ʼ���֤��", $"����˺��ʼ���֤��Ϊ:{code},��ע�Ᵽ��Ŷ��"), code);
#else
            var res = new EmailSendedResual(HttpStatusCode.OK, null, null);
            return await Task.FromResult((res, 197238));
#endif
        }
    }
    
}
