using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_DbLog))]
    public class Poe_DbLog
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Identity { get; set; }

        [StringLength(100)]
        public string User { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

    }
}