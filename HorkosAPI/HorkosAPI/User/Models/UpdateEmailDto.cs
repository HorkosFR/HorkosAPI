using System.Text.Json.Serialization;

namespace HorkosAPI.User.Models;

public class UpdateEmailDto
{
    public string Email { get; set; }

    public string Token { get; set; }

    public UpdateEmailDto() { }

    public UpdateEmailDto(string email, string token)
    {
        Email = email;
        Token = token;
    }

}
