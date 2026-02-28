using System.ComponentModel.DataAnnotations;

namespace HorkosAPI.Database.Models
{
    public class Level
    {
        [Key]
        public int LevelNumber { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public int PointsRequired { get; set; }

        public double VoteWeight { get; set; } = 1.0;
    }
}
