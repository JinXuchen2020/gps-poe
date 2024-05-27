using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Models
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }

        public string ImapServer { get; set; }
        public int ImapPort { get; set; }


        public string Account { get; set; }
        public string Password { get; set; }

        public EmailConfiguration()
        {
            SmtpServer = "smtp.office365.com";
            SmtpPort = 587;
            ImapServer = "smtp.office365.com";
            ImapPort = 993;
        }

        public EmailConfiguration(string account, string password)
        {
            SmtpServer = "exmail.beyondsoft.com";
            SmtpPort = 465;
            ImapServer = "exmail.beyondsoft.com";
            ImapPort = 993;
            Account = account;
            Password = password;
        }

        public EmailConfiguration(string account, string password, string smtpServer, int smtpPort, string imapServer, int imapPort)
        {
            Account = account;
            Password = password;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            ImapServer = imapServer;
            ImapPort = imapPort;
        }
    }
}
