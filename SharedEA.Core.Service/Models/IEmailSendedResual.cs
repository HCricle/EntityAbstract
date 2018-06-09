using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SharedEA.Core.Service.Models
{
    public interface IEmailSendedResual
    {
        HttpContent Body { get; }
        HttpResponseHeaders Headers { get; }
        HttpStatusCode StatusCode { get; }
    }
}