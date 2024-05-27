using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_MailTemplate))]
    public class Poe_MailTemplate : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(100)]
        public string IncentiveId { get; set; }

    }
}