using SharedEA.Core.Service.Models;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedEA.Core.Service
{
    public interface IEmailSenderService
    {
        Task<IEmailSendedResual> SendEmailAsync(string email, string subject, string message);
    }
}
