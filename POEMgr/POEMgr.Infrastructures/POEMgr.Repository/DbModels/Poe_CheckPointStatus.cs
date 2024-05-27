using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_CheckPointStatus))]
    public class Poe_CheckPointStatus : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(100)]
        public string PoeRequestId { get; set; }

        [StringLength(100)]
        public string CheckPointId { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

    }
}