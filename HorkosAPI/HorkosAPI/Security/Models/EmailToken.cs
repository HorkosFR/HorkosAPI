using System.IdentityModel.Tokens.Jwt;

namespace HorkosAPI.Security.Models;

public class EmailToken
{
    public string Value { get; set; }
    public string TokenType { get; set; }
    public bool IsActive { get; set; }

    public string Iss { get; set; }
    public string Aud { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public EmailToken(string token)
    {
        Value = token;

        JwtSecurityToken securityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);

        string tokenUserId = securityToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "";
        UserId = new Guid(tokenUserId);

        Username = securityToken.Claims.FirstOrDefault(x => x.Type == "Username")?.Value ?? "";
        Email = securityToken.Claims.FirstOrDefault(x => x.Type == "Email")?.Value ?? "";

        TokenType = securityToken.Claims.FirstOrDefault(x => x.Type == "TokenType")?.Value ?? "";

        string tokenExpirationDate = securityToken.Claims.FirstOrDefault(x => x.Type == "exp")?.Value ?? "";
        ExpirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExpirationDate)).DateTime;

        string tokenIssuedDate = securityToken.Claims.FirstOrDefault(x => x.Type == "iat")?.Value ?? "";
        IssuedDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExpirationDate)).DateTime;

        Iss = securityToken.Claims.FirstOrDefault(x => x.Type == "iss")?.Value ?? "";

        Aud = securityToken.Claims.FirstOrDefault(x => x.Type == "aud")?.Value ?? "";
    }
}
