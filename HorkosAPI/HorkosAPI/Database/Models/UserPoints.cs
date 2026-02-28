using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorkosAPI.Database.Models
{
    public class UserPoints
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public double TotalPoints { get; set; } = 0;

        public int CurrentLevel { get; set; } = 1;

        public DateTime LastUpdated { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; }

        [NotMapped]
        public virtual Level Level { get; set; }
        [NotMapped]
        public virtual Level NextLevel { get; set; }
        
        public UserPoints(User user)
        {
            Id = Guid.NewGuid();
            UserId = user.Id;
            TotalPoints = 0;
            CurrentLevel = 1;
        }

        public UserPoints() { }
    }

}
