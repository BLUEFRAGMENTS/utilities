using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bluefragments.Utilities.Smtp
{
    public interface IMailClient
    {
        Task<bool> SendMailAsync(HttpClient httpClient, string receivers, string subject, string body, byte[] attachment = null, string attachmentName = null);
    }
}
