using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Sys_UserRole))]
    public class Sys_UserRole : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }

        [StringLength(100)]
        public string UserID { get; set; }

        [StringLength(100)]
        public string RoleID { get; set; }

    }
}