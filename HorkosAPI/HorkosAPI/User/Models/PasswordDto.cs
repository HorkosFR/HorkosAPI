using System.Text.Json.Serialization;

namespace HorkosAPI.User.Models;

public class PasswordDto
{
    public string Password { get; set; }

    public string Token { get; set; }

    public PasswordDto() { }

    public PasswordDto(string password, string token)
    {
        Password = password;
        Token = token;
    }

}
