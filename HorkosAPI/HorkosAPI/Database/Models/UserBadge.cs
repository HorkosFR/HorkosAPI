using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorkosAPI.Database.Models
{
    public class UserBadge
    {
        [Key]
        public Guid BadgeId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Badge Badge { get; set; }
    }
}
