using FileService;
using FileService.Models;
using LogService;
using MailService;
using Microsoft.AspNetCore.Http;
using POEMgr.Application.Interfaces;
using POEMgr.Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application.Services
{
    internal class PoeLogService : IPoeLogService
    {
        private readonly LogCommon _logCommon;

        public PoeLogService(POEContext POEContext)
        {
            _logCommon = new LogCommon(POEContext);
        }

        public void WriteDbLog(DbLogType type, string identity, string user, string content)
        {
            _logCommon.WriteDbLog(type, identity, user, content);
        }
    }
}
