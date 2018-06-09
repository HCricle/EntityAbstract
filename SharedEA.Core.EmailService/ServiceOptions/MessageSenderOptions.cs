using SharedEA.Server.ServiceOptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.Service.ServiceOptions
{
    public class MessageSenderOptions:Options
    {
        public MessageSenderOptions()
        {
        }

        public MessageSenderOptions(string sendGridUser, string sendGridKey)
        {
            SendGridUser = sendGridUser;
            SendGridKey = sendGridKey;
        }

        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}
