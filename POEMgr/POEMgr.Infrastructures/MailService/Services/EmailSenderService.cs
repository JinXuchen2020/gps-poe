using MailKit.Net.Smtp;
using MailService.Interfaces;
using MailService.Models;
using MimeKit;
using Newtonsoft.Json;

namespace MailService.Services
{
    public class EmailSenderService: IEmailSenderService
    {
        private readonly DNSSmtpClient _client;
        private readonly EmailConfiguration _configuration;

        public EmailSenderService(EmailConfiguration emailConfiguration)
        {
            try
            {
                _configuration = emailConfiguration;
                _client = new DNSSmtpClient();
                Initialize();
            }
            catch (Exception)
            {
            }
        }

        public EmailSenderService(string address, string password)
        {
            try
            {
                _configuration = new EmailConfiguration(address, password);
                _client = new DNSSmtpClient();
                this.Initialize();
            }
            catch (Exception)
            {
            }
        }

        public async Task<string> SendAsync(string to, string subject, string body)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(_configuration.Account)
                .To(to)
                .Subject(subject)
                .Body(body)
                .Build();

            return await SendAsync(message);
        }

        public async Task<string> SendAsync(List<string> to, string subject, string body)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(_configuration.Account)
                .To(to)
                .Subject(subject)
                .Body(body)
                .Build();

            return await SendAsync(message);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(_configuration.Account)
                .To(to)
                .Cc(cc)
                .Subject(subject)
                .Body(body)
                .Build();

            return await SendAsync(message);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<string> attachmentPath)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(_configuration.Account)
                .To(to)
                .Cc(cc)
                .Subject(subject)
                .Body(body, attachmentPath)
                .Build();

            return await SendAsync(message);
        }

        public async Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<(string, byte[])> attachments, List<(string, byte[])> linkedResources = null)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(_configuration.Account)
                .To(to)
                .Cc(cc)
                .Subject(subject)
                .Body(body, attachments, linkedResources)
                .Build();

            return await SendAsync(message);
        }

        public async Task<string> SendAsync(string from, List<string> to, List<string> cc, List<string> bcc, string subject, string body, List<string> attachmentPath)
        {
            MimeMessage message = new MimeMessageBuilder()
                .From(from)
                .To(to)
                .Cc(cc)
                .Bcc(bcc)
                .Subject(subject)
                .Body(body, attachmentPath)
                .Build();

            return await SendAsync(message);
        }

        private void Initialize()
        {
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(_configuration.SmtpServer, _configuration.SmtpPort, false);
            _client.Authenticate(_configuration.Account, _configuration.Password);
        }

        private async Task<string> SendAsync(MimeMessage message)
        {
            try
            {
                await _client.SendAsync(message);
                return string.Empty;
            }
            catch(SmtpCommandException ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = ex.ErrorCode.ToString(), EmailAddress = ex.Mailbox.Address, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorMessage = ex.Message });
            }
        }
    }
}
