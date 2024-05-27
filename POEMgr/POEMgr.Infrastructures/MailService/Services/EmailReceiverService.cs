using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MailService.Interfaces;
using MailService.Models;

namespace MailService.Services
{
    public class EmailReceiverService : IEmailReceiverService
    {
        private readonly IImapClient _client;
        private readonly EmailConfiguration _configuration;

        public EmailReceiverService(EmailConfiguration emailConfiguration)
        {
            try
            {
                _configuration = emailConfiguration;
                _client = new ImapClient();
                Initialize();
            }
            catch (Exception)
            {
            }
        }

        public EmailReceiverService(string address, string password)
        {
            try
            {
                _configuration = new EmailConfiguration(address, password);
                _client = new ImapClient();
                Initialize();
            }
            catch (Exception)
            {
            }
        }

        public List<Message> GetUnReadMessage(DateTime? deliveryDate = null)
        {
            List<Message> messages = new List<Message>();

            _client.Inbox.Open(FolderAccess.ReadWrite);

            var query = SearchQuery.NotSeen;

            var uids = _client.Inbox.Search(query);
            foreach (var uid in uids)
            {
                var message = _client.Inbox.GetMessage(uid);
                if (deliveryDate == null || (deliveryDate != null && message.Date > deliveryDate))
                {
                    messages.Add(new Message
                    {
                        Id = uid,
                        Tos = message.To.Mailboxes.Select(x => x.Address).ToList(),
                        Ccs = message.Cc.Mailboxes.Select(x => x.Address).ToList(),
                        Bccs = message.Bcc.Mailboxes.Select(x => x.Address).ToList(),
                        Title = message.Subject,
                        Content = message.HtmlBody ?? message.TextBody,
                    });
                }
            }

            _client.Inbox.SetFlags(messages.Select(c => c.Id).ToList(), MessageFlags.Seen, true);

            return messages;
        }

        public List<Message> GetMessageTitleContains(string text)
        {
            List<Message> messages = new List<Message>();

            _client.Inbox.Open(FolderAccess.ReadWrite);

            var uids = _client.Inbox.Search(SearchQuery.HeaderContains("Subject", text));
            foreach (var uid in uids)
            {
                var message = _client.Inbox.GetMessage(uid);
                messages.Add(new Message
                {
                    Id = uid,
                    Tos = message.To.Mailboxes.Select(x => x.Address).ToList(),
                    Ccs = message.Cc.Mailboxes.Select(x => x.Address).ToList(),
                    Bccs = message.Bcc.Mailboxes.Select(x => x.Address).ToList(),
                    Title = message.Subject,
                    Content = message.HtmlBody ?? message.TextBody,
                });
            }

            _client.Inbox.SetFlags(messages.Select(c => c.Id).ToList(), MessageFlags.Seen, true);

            return messages;
        }

        public List<Message> GetUnReadMessageTitleContains(string text)
        {
            List<Message> messages = new List<Message>();
            _client.Inbox.Open(FolderAccess.ReadWrite);

            var uids = _client.Inbox.Search(SearchQuery.And(SearchQuery.NotSeen, SearchQuery.HeaderContains("Subject", text)));
            foreach (var uid in uids)
            {
                var message = _client.Inbox.GetMessage(uid);
                if (message.Subject.Contains(text.Trim()))
                {
                    messages.Add(new Message
                    {
                        Id = uid,
                        Tos = message.To.Mailboxes.Select(x => x.Address).ToList(),
                        Ccs = message.Cc.Mailboxes.Select(x => x.Address).ToList(),
                        Bccs = message.Bcc.Mailboxes.Select(x => x.Address).ToList(),
                        Title = message.Subject,
                        Content = message.HtmlBody ?? message.TextBody,
                    });
                }
            }

            _client.Inbox.SetFlags(messages.Select(c => c.Id).ToList(), MessageFlags.Seen, true);

            return messages;
        }

        private void Initialize()
        {
            _client.Connect(_configuration.ImapServer, _configuration.ImapPort, SecureSocketOptions.SslOnConnect);
            _client.Authenticate(_configuration.Account, _configuration.Password);
        }

    }
}
