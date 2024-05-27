using POEMgr.Domain.Models;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;

namespace LogService
{
    public enum DbLogType
    { 
        Create,
        Read, 
        Update,
        Delete,
    }

    public enum OperationLogType
    {
        Create,
        Read,
        Update,
        Delete,
    }

    public class LogCommon
    {
        private readonly POEContext _POEContext;

        public LogCommon(POEContext poeContext)
        {
            _POEContext = poeContext;
        }

        public async void WriteOperationLog(OperationLogType type, string identity, string user, string content)
        {
            Poe_OperationLog poe_Log = new Poe_OperationLog {
                Id = Guid.NewGuid(),
                Identity = identity,
                Content = content,
                CreateTime = DateTime.Now,
                User = user,
                Type = type.ToString(),
            };
            await _POEContext.Poe_Log.AddAsync(poe_Log);
            _POEContext.SaveChanges();
        }

        public async void WriteDbLog(DbLogType type, string identity, string user, string content)
        {
            Poe_DbLog Poe_DbLog = new Poe_DbLog
            {
                Id = Guid.NewGuid(),
                Identity = identity,
                User = user,
                Content = content,
                Type= type.ToString(),
                CreateTime = DateTime.Now,
            };
            await _POEContext.Poe_DbLog.AddAsync(Poe_DbLog);
            _POEContext.SaveChanges();
        }
    }
}