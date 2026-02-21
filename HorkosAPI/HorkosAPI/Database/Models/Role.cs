namespace HorkosAPI.Database.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    public enum RoleValues
    {
        User,
        Moderator,
        Admin
    }
}
