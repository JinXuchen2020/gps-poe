using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Sys_User))]
    public class Sys_User: EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string Alias { get; set; }

        [StringLength(100)]
        public string Mail { get; set; }

        [StringLength(100)]
        public string IsDisabled { get; set; }
    }
}