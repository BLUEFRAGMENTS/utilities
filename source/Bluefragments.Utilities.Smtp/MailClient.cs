using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bluefragments.Utilities.Smtp
{
    public class MailClient : IMailClient
    {
        private readonly string apiKey;
        private readonly string fromName;
        private readonly string fromEmail;

        public MailClient(string apiKey, string fromName, string fromEmail)
        {
            apiKey.ThrowIfParameterIsNullOrWhiteSpace(nameof(apiKey));
            fromName.ThrowIfParameterIsNullOrWhiteSpace(nameof(fromName));
            fromEmail.ThrowIfParameterIsNullOrWhiteSpace(nameof(fromEmail));

            this.apiKey = apiKey;
            this.fromName = fromName;
            this.fromEmail = fromEmail;
        }

        public static byte[] ReadAttachment(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async Task<bool> SendMailAsync(string receivers, string subject, string body, byte[] attachment = null, string attachmentName = null, HttpClient httpClient = null)
        {
            if (string.IsNullOrEmpty(receivers))
            {
                throw new ArgumentNullException(nameof(receivers));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            var client = new SendGridClient(httpClient, apiKey: apiKey);

            body = body.Replace("NEWLINE", Environment.NewLine);
            var plainBody = "Content is only available as HTML";
            var from = new EmailAddress(fromEmail, fromName);

            var to = new List<EmailAddress>();
            foreach (var receiver in receivers.Split(';'))
            {
                to.Add(new EmailAddress(receiver));
            }

            var message = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, plainBody, body);

            if (attachment != null)
            {
                var file = Convert.ToBase64String(attachment);
                message.AddAttachment(attachmentName, file);
            }

            var response = await client.SendEmailAsync(message);
            return response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
