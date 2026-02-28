using System.ComponentModel.DataAnnotations;

namespace HorkosAPI.Database.Models
{
    public class GamificationAction
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string ActionCode { get; set; }

        [Required]
        public string Description { get; set; }

        public double Points { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
