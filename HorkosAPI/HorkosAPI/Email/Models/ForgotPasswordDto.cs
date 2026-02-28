using System.Text.Json.Serialization;

namespace HorkosAPI.Email.Models;

public class ForgotPasswordDto
{
    public string Email { get; set; }

    public ForgotPasswordDto(string email)
    {
        Email = email;
    }
}
