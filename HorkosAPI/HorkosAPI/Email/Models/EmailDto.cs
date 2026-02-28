using System.Text.Json.Serialization;

namespace HorkosAPI.Email.Models;

public class EmailDto
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    public EmailDto(string username, string email)
    {
        Username = username;
        Email = email;
    }
}
