using MailService.Models;

namespace MailService.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendAsync(string to, string subject, string body);
        Task<string> SendAsync(List<string> to, string subject, string body);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<string> attachmentPath);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<(string, byte[])> attachments, List<(string, byte[])> linkedResources = null);
        Task<string> SendAsync(string from, List<string> to, List<string> cc, List<string> bcc, string subject, string body, List<string> attachmentPath);


        List<Message> GetUnReadMessage(DateTime? deliveryDate = null);
        List<Message> GetMessageTitleContains(string text);
        List<Message> GetUnReadMessageTitleContains(string text);
    }
}
