using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_RequestPhase))]
    public class Poe_RequestPhase : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string IncentiveId { get; set; }

        [StringLength(100)]
        public string FiscalYear { get; set; }

        [StringLength(100)]
        public string FiscalQuater { get; set; }

        [StringLength(100)]
        public string FiscalMonth { get; set; }

    }
}