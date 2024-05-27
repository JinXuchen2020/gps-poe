using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Repository.DbModels
{
    public class EntityBase
    {
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? ModifiedTime { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}
