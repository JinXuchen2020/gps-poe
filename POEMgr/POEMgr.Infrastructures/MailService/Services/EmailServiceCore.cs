using MailService.Interfaces;
using MailService.Models;
using System.Diagnostics;

namespace MailService.Services
{
    public class EmailServiceCore : IEmailService
    {
        private readonly IEmailSenderService _emailSender;
        private readonly IEmailReceiverService _emailReceiver;

        public EmailServiceCore(EmailConfiguration emailConfiguration)
        {
            _emailReceiver = new EmailReceiverService(emailConfiguration);
            _emailSender = new EmailSenderService(emailConfiguration);
        }

        public EmailServiceCore(string address, string password)
        {
            _emailReceiver = new EmailReceiverService(address, password);
            _emailSender = new EmailSenderService(address, password);
        }


        public List<Message> GetMessageTitleContains(string text)
        {
            return this._emailReceiver.GetMessageTitleContains(text);
        }

        public List<Message> GetUnReadMessage(DateTime? deliveryDate = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var result = this._emailReceiver.GetUnReadMessage(deliveryDate);
            while (result.Count == 0 && sw.Elapsed < TimeSpan.FromMinutes(2))
            {
                System.Threading.Thread.Sleep(3000);
                result = this._emailReceiver.GetUnReadMessage(deliveryDate);
            }

            sw.Stop();
            return result;
        }

        public List<Message> GetUnReadMessageTitleContains(string text)
        {
            return this._emailReceiver.GetUnReadMessageTitleContains(text);
        }

        public async Task<string> SendAsync(string to, string subject, string body)
        {
            return await this._emailSender.SendAsync(to, subject, body);
        }

        public async Task<string> SendAsync(List<string> to, string subject, string body)
        {
            return await this._emailSender.SendAsync(to, subject, body);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body)
        {
            return await this._emailSender.SendAsync(to, cc, subject, body);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<string> attachmentPath)
        {
            return await this._emailSender.SendAsync(to, cc, subject, body, attachmentPath);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<(string, byte[])> attachments, List<(string, byte[])> linkedResources = null)
        {
            return await this._emailSender.SendAsync(to, cc, subject, body, attachments, linkedResources);
        }

        public async Task<string> SendAsync(string from, List<string> to, List<string> cc, List<string> bcc, string subject, string body, List<string> attachmentPath)
        {
            return await this._emailSender.SendAsync(from, to, cc, bcc, subject, body, attachmentPath);
        }
    }
}
