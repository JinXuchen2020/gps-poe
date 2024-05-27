using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_CurrentNumber))]
    public class Poe_CurrentNumber
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string CurrentNumber { get; set; }

    }
}