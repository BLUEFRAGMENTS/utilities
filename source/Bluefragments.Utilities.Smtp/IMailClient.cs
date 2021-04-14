using System.Net.Http;
using System.Threading.Tasks;

namespace Bluefragments.Utilities.Smtp
{
    public interface IMailClient
    {
        Task<bool> SendMailAsync(string receivers, string subject, string body, byte[] attachment = null, string attachmentName = null, HttpClient httpClient = null);
    }
}
