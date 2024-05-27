using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POEMgr.Application.TransferModels
{
    public class ApiResult
    {
        public int Code { get; set; }= 0;
        public string Msg { get; set; } = "";
        public dynamic Data { get; set; }
    }
}
