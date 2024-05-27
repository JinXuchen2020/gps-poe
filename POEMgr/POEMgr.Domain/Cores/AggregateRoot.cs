using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Domain.Cores
{
    public class AggregateRoot : Entity, IAggregateRoot
    {
        public DateTime CreatedTime { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedTime { get; set; }
        public Guid ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
