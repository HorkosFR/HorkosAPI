using System.Text.Json.Serialization;

namespace HorkosAPI.Auth.Models;

public class VerificationDto
{
    [JsonPropertyName("email_token")]
    public required string EmailToken { get; set; }

    [JsonPropertyName("init")]
    public bool IsFirstVerification { get; set; }
}
