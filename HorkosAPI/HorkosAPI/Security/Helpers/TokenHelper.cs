using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HorkosAPI.Security.Helpers;

public static class TokenHelper
{
    public static string WriteTokenWithSecurityKey(this JwtSecurityTokenHandler token, SymmetricSecurityKey key, string host, IEnumerable<Claim> claims, int duration = 3)
        => token.WriteToken(new JwtSecurityToken(
                        new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256)),
                        new JwtPayload(host, host, claims, null, DateTime.UtcNow.AddHours(duration), DateTime.UtcNow)
                    )
              );
}
