using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HorkosAPI.Database.Services;
using HorkosAPI.Global;
using HorkosAPI.Security.Models;
using HorkosAPI.Security.Models.Enumerations;
using HorkosAPI.User.Models.Enumerations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;

namespace HorkosAPI.Security.Services;

public class UserTokenService(IConfiguration configuration, DatabaseContext databaseContext) : IUserTokenService
{

    public async Task<string> CheckRefreshToken(string refreshToken)
    {
        refreshToken = refreshToken.Replace("Bearer ", null);
        JwtSecurityTokenHandler tokenHandler = new();
        if (!tokenHandler.CanReadToken(refreshToken))
            throw new Exception(UserTokenResponse.WrongTokenFormat.ToString());

        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(refreshToken);

        string tokenUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "";
        Guid userId = new(tokenUserId);
        Database.Models.User user = databaseContext.Users
            .First(x => x.Id == userId)
            .EnsureNotNull(UserResponse.UserDoesNotExist.ToString());

        var stringToken = Convert.FromBase64String(refreshToken);
        var hashToken = Crypto.HashPassword(BitConverter.ToString(stringToken));
        if (hashToken != user.RefreshToken)
        {
            throw new Exception(UserTokenResponse.InvalidUserRefreshToken.ToString());
        }

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateLifetime = false,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = "appOrigin",
            ValidAudience = "appOrigin",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Application:OAuth2").Value ?? "")),
            ClockSkew = TimeSpan.FromSeconds(0.0)
        };

        try
        {
            tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
            return GenerateJwtToken(user, DateTime.UtcNow.AddHours(2), "Access");
        }
        catch (Exception ex)
        {
            throw new Exception(UserTokenResponse.InvalidUserAccessToken.ToString(), ex);
        }
    }


    public async Task<string> CheckAccessToken(string userToken)
    {
        userToken = userToken.Replace("Bearer ", null);
        JwtSecurityTokenHandler tokenHandler = new();
        if (!tokenHandler.CanReadToken(userToken))
            throw new Exception(UserTokenResponse.WrongTokenFormat.ToString());
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(userToken);

        string tokenTypeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "TokenType")?.Value ?? "";
        if (tokenTypeClaim != "Access")
            throw new Exception(UserTokenResponse.NotAcessTypeToken.ToString());

        string tokenUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "";
        Guid userId = new(tokenUserId);
        Database.Models.User user = databaseContext.Users
            .FirstOrDefault(x => x.Id == userId)
            .EnsureNotNull(UserResponse.UserDoesNotExist.ToString());

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = "appOrigin",
            ValidAudience = "appOrigin",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Application:OAuth2").Value ?? "")),
            ClockSkew = TimeSpan.FromSeconds(0.0)
        };

        try
        {
            tokenHandler.ValidateToken(userToken, tokenValidationParameters, out SecurityToken validatedToken);

            return userToken;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new Exception(UserTokenResponse.ExpiredUserAccessToken.ToString());
        }
        catch (Exception)
        {
            throw new Exception(UserTokenResponse.InvalidUserAccessToken.ToString());
        }
    }

    public string GenerateJwtToken(Database.Models.User user, DateTime? expirationDate, string tokenType)
    {
        var issuer = "appOrigin";

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(new JwtSecurityToken(
             new JwtHeader(
                 new SigningCredentials(
                     new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(configuration.GetSection("Application:OAuth2").Value ?? "")),
                     SecurityAlgorithms.HmacSha256)),
             new JwtPayload(
                 issuer,
                 issuer,
                 new[]
                 {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("TokenType", tokenType),
                    new Claim("Scope" , "all")
                 },
                 null,
                 DateTime.UtcNow,
                 expirationDate
                 )
             ));
    }
}
