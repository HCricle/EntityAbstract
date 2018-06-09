using SharedEA.Core.Service.Models;
using SharedEA.Core.Service.ServiceOptions;
using SharedEA.Server.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedEA.Core.Service
{
    public class EmailSenderService :IEmailSenderService,IService
    {
        public static readonly string EmailApiKey = "";
        public EmailSenderService()
        {
            Options = new MessageSenderOptions("Ea Team", EmailApiKey);
        }

        public MessageSenderOptions Options { get; } //set only via Secret Manager

        public async Task<IEmailSendedResual> SendEmailAsync(string email, string subject, string message)
        {
#if !DEBUG
            return await ExecuteAsync(Options.SendGridKey, subject, message, email);
#else
            return await Task.FromResult(new EmailSendedResual(System.Net.HttpStatusCode.OK, null, null));
#endif

        }

        private async Task<IEmailSendedResual> ExecuteAsync(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("EaTeam@SharedEA.Core.com", "EaTeam"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            var res= await client.SendEmailAsync(msg);
            return new EmailSendedResual(res.StatusCode, res.Body, res.Headers);
        }
    }
}

