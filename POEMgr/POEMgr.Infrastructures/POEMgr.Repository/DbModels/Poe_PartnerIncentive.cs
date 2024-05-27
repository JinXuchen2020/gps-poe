using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_PartnerIncentive))]
    public class Poe_PartnerIncentive : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string PartnerId { get; set; }

        [StringLength(100)]
        public string IncentiveId { get; set; }

        [StringLength(4000)]
        public string Mail { get; set; }

        [StringLength(4000)]
        public string MailCC { get; set; }
    }
}
