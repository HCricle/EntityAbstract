using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SharedEA.Core.Service.Models
{
    public class EmailSendedResual : IEmailSendedResual
    {
        public EmailSendedResual(HttpStatusCode statusCode, HttpContent body, HttpResponseHeaders headers)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = headers;
        }

        public HttpStatusCode StatusCode { get; }
        public HttpContent Body { get;  }
        public HttpResponseHeaders Headers { get;  }
    }
}
