using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_SubscriptionStatus))]
    public class Poe_SubscriptionStatus : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string PoeRequestId { get; set; }

        [StringLength(100)]
        public string SubscriptionId { get; set; }

        [StringLength(100)]
        public string Status { get; set; }
    }
}
