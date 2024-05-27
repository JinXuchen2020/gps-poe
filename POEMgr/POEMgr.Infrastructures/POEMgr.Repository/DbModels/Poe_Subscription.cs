using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_Subscription))]
    public class Poe_Subscription : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string CustomerId { get; set; }

        [StringLength(100)]
        public string Status { get; set; }
    }
}