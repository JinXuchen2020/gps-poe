using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_SchedulerLog))]
    public class Poe_SchedulerLog
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(4000)]
        public string ErrorMsg { get; set; }

        public DateTime CreateTime { get; set; }

    }
}