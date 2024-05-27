using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEMgr.Repository.DbModels
{
    [Table(nameof(Poe_POEFile))]
    public class Poe_POEFile : EntityBase
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; }


        [StringLength(100)]
        public string IncentiveId { get; set; }

        [StringLength(100)]
        public string PoeRequestId { get; set; }

        [StringLength(100)]
        public string FileName { get; set; }

        [StringLength(500)]
        public string Path { get; set; }

        [StringLength(500)]
        public string BlobUri { get; set; }

    }
}