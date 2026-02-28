using System.ComponentModel.DataAnnotations;

namespace HorkosAPI.Database.Models
{
    public class Badge
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string BadgeCode { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int PointsBonus { get; set; } = 0;
        public string Icon { get; set; }
        public ICollection<UserBadge> Users { get; set; } = new List<UserBadge>();

    }
}
