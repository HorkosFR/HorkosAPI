namespace HorkosAPI.User.Models;

public class UserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime? LastModificationDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? ImageUrl { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageMimeType { get; set; }
    public string Locale { get; set; }

    public UserDto(string username, string email)
    {
        Username = username;
        Email = email;
        CreatedDate = new DateTime();
    }

    public UserDto(Database.Models.User dbUser)
    {
        Username = dbUser.Username;
        Email = dbUser.Email;
        CreatedDate = dbUser.CreatedDate;
    }


    public UserDto(Database.Models.User dbUser, string roleName)
    {
        Username = dbUser.Username;
        Email = dbUser.Email;
        CreatedDate = dbUser.CreatedDate;
    }
}
