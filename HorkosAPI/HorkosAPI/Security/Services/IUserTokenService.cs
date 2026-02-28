namespace HorkosAPI.Security.Services;

public interface IUserTokenService
{
    Task<string> CheckRefreshToken(string refreshToken);
    Task<string> CheckAccessToken(string accessToken);
    string GenerateJwtToken(Database.Models.User user, DateTime? expirationDate, string tokenType);
}
