using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merino.Entity
{
    public class BaseEntity
    {

        [Column("create_pgm_id")]
        [Required]
        public string? CreatePgmId { get; set; }

        [Column("create_user_id")]
        [Required]
        public string? CreateUserId { get; set; }

        [Column("create_date")]
        [Required]
        public DateTime CreateDate { get; set; }

        [Column("update_pgm_id")]
        [Required]
        public string? UpdatePgmId { get; set; }

        [Column("update_user_id")]
        [Required]
        public string? UpdateUserId { get; set; }

        [Column("update_date")]
        [Required]
        public DateTime UpdateDate { get; set; }

    }
}
