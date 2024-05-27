using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application.TransferModels
{
    public class PoeRequest_email_send_req
    {
        public List<string> RequestIds { get; set; }

        public string Type { get; set; }
    }
}
