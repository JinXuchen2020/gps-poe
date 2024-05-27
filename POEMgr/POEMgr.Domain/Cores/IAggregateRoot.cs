using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Domain.Cores
{
    public interface IAggregateRoot
    {
        DateTime CreatedTime { get; set; }
        Guid CreatedBy { get; set; }
        DateTime ModifiedTime { get; set; }
        Guid ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
