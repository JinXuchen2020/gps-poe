using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application.Interfaces
{
    public interface IPoeEmailService
    {
        Task<string> SendAsync(string to, string subject, string body);
        Task<string> SendAsync(List<string> to, string subject, string body);
        Task<string> SendAsync(List<string> to, List<string> cc, string subject, string body);

        Task<string> SendNotifyEmailAsync(string requestId, string type);
    }
}
