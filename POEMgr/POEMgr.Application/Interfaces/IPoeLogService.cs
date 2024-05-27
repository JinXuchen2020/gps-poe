using FileService.Models;
using LogService;
using Microsoft.AspNetCore.Http;

namespace POEMgr.Application.Interfaces
{
    public interface IPoeLogService
    {
        public void WriteDbLog(DbLogType type, string identity, string user, string content);
    }
}
