using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_MailSendRecord))]
    public class Poe_MailSendRecord : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string PoeRequestId { get; set; }

        [StringLength(4000)]
        public string SendTo { get; set; }

        [StringLength(4000)]
        public string CC { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(4000)]
        public string Content { get; set; }

        [StringLength(4000)]
        public string ErrorMsg { get; set; }

    }
}