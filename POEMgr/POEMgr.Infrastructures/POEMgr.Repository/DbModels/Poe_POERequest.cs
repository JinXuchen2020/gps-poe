using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_POERequest))]
    public class Poe_POERequest : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string IncentiveId { get; set; }

        [StringLength(100)]
        public string PhaseId { get; set; }

        [StringLength(100)]
        public string PartnerId { get; set; }

        [StringLength(100)]
        public string CustomerId { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DeadLineDate { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}