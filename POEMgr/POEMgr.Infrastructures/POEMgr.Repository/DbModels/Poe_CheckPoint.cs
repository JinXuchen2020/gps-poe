using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_CheckPoint))]
    public class Poe_CheckPoint : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string IncentiveId { get; set; }

        [StringLength(100)]
        public string Content { get; set; }

    }
}