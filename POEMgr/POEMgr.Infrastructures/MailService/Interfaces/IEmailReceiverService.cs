using MailService.Models;

namespace MailService.Interfaces
{
    public interface IEmailReceiverService
    {
        List<Message> GetUnReadMessage(DateTime? deliveryDate = null);
        List<Message> GetMessageTitleContains(string text);
        List<Message> GetUnReadMessageTitleContains(string text);
    }
}
