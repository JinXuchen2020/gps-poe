using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_Incentive))]
    public class Poe_Incentive : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string ProgameName { get; set; }

        [StringLength(100)]
        public string IncentiveName { get; set; }

        [StringLength(6000)]
        public string WelcomeSpeech { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SubmitDeadlineDay { get; set; }
        public int? RemindEmailDay { get; set; }
        public int? ReSubmitDeadlineDay { get; set; }
        public int? RejectCount { get; set; }


    }
}