using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Interfaces
{
    public interface IEmailSenderService
    {
        Task<string> SendAsync(string to, string subject, string body);
        Task<string> SendAsync(List<string> to, string subject, string body);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<string> attachmentPath);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body, List<(string, byte[])> attachments, List<(string, byte[])> linkedResources = null);
        Task<string> SendAsync(string from, List<string> to, List<string> cc, List<string> bcc, string subject, string body, List<string> attachmentPath);
    }
}
