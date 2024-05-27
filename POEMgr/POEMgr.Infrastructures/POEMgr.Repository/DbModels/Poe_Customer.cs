using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_Customer))]
    public class Poe_Customer : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string PartnerId { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

    }
}