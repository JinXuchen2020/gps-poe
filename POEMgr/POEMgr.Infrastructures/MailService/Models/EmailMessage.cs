using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;

namespace MailService.Models
{
    public class Message
    {
        public UniqueId Id { get; set; }
        public List<string> Tos { get; set; }
        public List<string> Ccs { get; set; }
        public List<string> Bccs { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Attachments { get; set; }
    }
}
