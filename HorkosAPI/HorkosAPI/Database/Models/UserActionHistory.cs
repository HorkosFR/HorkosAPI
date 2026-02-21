using System.ComponentModel.DataAnnotations;

namespace HorkosAPI.Database.Models
{
    public class UserActionHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ActionId { get; set; }

        public Guid? ContributionId { get; set; }

        public double PointsAwarded { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual GamificationAction Action { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual UserContribution Contribution { get; set; }
    }
}
