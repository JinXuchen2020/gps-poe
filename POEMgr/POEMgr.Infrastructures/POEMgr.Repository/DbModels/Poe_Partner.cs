using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_Partner))]
    public class Poe_Partner : EntityBase
    {
        [Key]
        public string Id { get; set; }

        [StringLength(100)]
        public string PartnerOneId { get; set; }

        [StringLength(100)]
        public string PartnerName { get; set; }

        [StringLength(4000)]
        public string Mail { get; set; }

        [StringLength(4000)]
        public string MailCC { get; set; }

        [StringLength(100)]
        public string PhaseId { get; set; }

        [StringLength(100)]
        public string IsDisabled { get; set; }

    }
}