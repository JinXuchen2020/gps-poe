using MimeKit;

namespace MailService.Services
{
    internal class MimeMessageBuilder
    {
        private MimeMessage _mimeMessage { get; set; }

        internal MimeMessageBuilder()
        {
            Reset();
        }

        internal void Reset()
        {
            _mimeMessage = new MimeMessage();
        }

        internal MimeMessageBuilder From(string address)
        {
            _mimeMessage.From.Add(MailboxAddress.Parse(address));
            return this;
        }

        internal MimeMessageBuilder Subject(string subject)
        {
            _mimeMessage.Subject = subject;
            return this;
        }

        internal MimeMessageBuilder To(string address)
        {
            _mimeMessage.To.Add(MailboxAddress.Parse(address));
            return this;
        }

        internal MimeMessageBuilder To(List<string> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => MailboxAddress.Parse(x));
            _mimeMessage.To.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder To(List<(string name, string address)> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => new MailboxAddress(x.name, x.address));
            _mimeMessage.To.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder Cc(string address)
        {
            _mimeMessage.Cc.Add(MailboxAddress.Parse(address));
            return this;
        }

        internal MimeMessageBuilder Cc(List<string> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => MailboxAddress.Parse(x));
            _mimeMessage.Cc.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder Cc(List<(string name, string address)> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => new MailboxAddress(x.name, x.address));
            _mimeMessage.Cc.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder Bcc(string address)
        {
            _mimeMessage.Bcc.Add(MailboxAddress.Parse(address));
            return this;
        }

        internal MimeMessageBuilder Bcc(List<string> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => MailboxAddress.Parse(x));
            _mimeMessage.Bcc.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder Bcc(List<(string name, string address)> addresses)
        {
            IEnumerable<MailboxAddress> mailboxAddresses = addresses.Select(x => new MailboxAddress(x.name, x.address));
            _mimeMessage.Bcc.AddRange(mailboxAddresses);
            return this;
        }

        internal MimeMessageBuilder Body(string body)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            _mimeMessage.Body = bodyBuilder.ToMessageBody();
            return this;
        }

        internal MimeMessageBuilder Body(string body, List<string> attachmentPath)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            bodyBuilder.HtmlBody = body;
            attachmentPath?.ForEach(x => bodyBuilder.Attachments.Add(x));
            _mimeMessage.Body = bodyBuilder.ToMessageBody();
            return this;
        }

        internal MimeMessageBuilder Body(string body, List<(string, byte[])> attachments, List<(string, byte[])> linkedResources = null)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            bodyBuilder.HtmlBody = body;
            attachments?.ForEach(x => bodyBuilder.Attachments.Add(x.Item1, x.Item2));
            linkedResources?.ForEach(x => bodyBuilder.LinkedResources.Add(x.Item1, x.Item2));
            _mimeMessage.Body = bodyBuilder.ToMessageBody();
            return this;
        }

        internal MimeMessage Build()
        {
            return _mimeMessage;
        }

    }
}