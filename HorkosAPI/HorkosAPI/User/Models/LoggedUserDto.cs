namespace HorkosAPI.User.Models;

public class LoggedUserDto
{
    public string Username { get; set; }
    public Guid UserId { get; set; }

    public string Email { get; set; }
    public string RoleName { get; set; }
    public string? UserToken { get; set; }
    public string? UserImage { get; set; }
    public string Locale { get; set; }

    public LoggedUserDto(Database.Models.User dbUser, string roleName, string? userToken = null)
    {
        UserId = dbUser.Id;
        Username = dbUser.Username;
        Email = dbUser.Email;
        Locale= dbUser.Locale;
        RoleName = roleName;
        UserToken = userToken;
        UserImage = dbUser.ImageUrl;
    }

    public LoggedUserDto() { }
}
