using HorkosAPI.Auth.Models;
using HorkosAPI.User.Models;

namespace HorkosAPI.Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public Guid RoleId { get; set; }
    public string? Password { get; set; }
    public DateTime? LastModificationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLogin { get; set; }
    public virtual Role? Role { get; set; }
    public string RefreshToken { get; set; }
    public ICollection<UserContribution> Contributions { get; set; } = new List<UserContribution>();
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();
    public UserPoints Points { get; set; }
    public string Locale { get; set; }
    public string AccountStatus { get; set; }
    public string? ImageUrl { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageMimeType { get; set; }

    public User() { }
    public User(UserDto dto, Guid roleId)
    {
        Id = Guid.NewGuid();
        Username = dto.Username;
        Email = dto.Email;
        Password = dto.Password;
        Locale = dto.Locale;
        CreatedDate = dto.CreatedDate == default ? DateTime.UtcNow : dto.CreatedDate;
        LastModificationDate = dto.LastModificationDate ?? DateTime.UtcNow;

        LastLogin = DateTime.UtcNow;
        RoleId = roleId;

        ImageUrl = dto.ImageUrl;
        ImageData = dto.ImageData;
        ImageMimeType = dto.ImageMimeType;

        RefreshToken = string.Empty;
        Contributions = new List<UserContribution>();
        Comments = new List<Comment>();
    }

    public User(RegisterDto dto, Guid roleId)
    {
        Id = Guid.NewGuid();
        Username = dto.Username;
        Email = dto.Email;
        Password = dto.Password;
        CreatedDate = DateTime.UtcNow;
        LastModificationDate = DateTime.UtcNow;
        LastLogin = DateTime.UtcNow;
        RoleId = roleId;
        AccountStatus = "New";

        RefreshToken = string.Empty;
        Contributions = new List<UserContribution>();
        Comments = new List<Comment>();
    }
}

public enum AccountStatus
{
    RegistrationInitialized,
    EmailVerified,
    RegistrationSucceeded,
    Blocked
}