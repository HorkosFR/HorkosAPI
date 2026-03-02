namespace HorkosAPI.Security.Services;

public interface IUserTokenService
{
    Task<string> CheckRefreshToken(string refreshToken);
    Task<string> CheckAccessToken(string accessToken, bool checkOld = false);
    string GenerateJwtToken(Database.Models.User user, DateTime? expirationDate, string tokenType);
}
