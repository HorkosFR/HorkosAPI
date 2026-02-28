namespace HorkosAPI.User.Models
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public string Locale { get; set; }
    }
}
