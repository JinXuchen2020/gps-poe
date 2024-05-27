using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_RequestLog))]
    public class Poe_RequestLog : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string RequestId { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(4000)]
        public string Reason { get; set; }

    }
}