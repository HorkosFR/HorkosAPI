using Microsoft.Net.Http.Headers;
using HorkosAPI.Global;
using HorkosAPI.Role.Services;
using HorkosAPI.Security.Models.Enumerations;
using HorkosAPI.Security.Services;
using HorkosAPI.User.Services;

namespace HorkosAPI.Security.Middleware;

public class UserTokenMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] PublicRoutes =
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/verify",
        "/api/auth/resend",
        "/api/auth/update-password",
        "/api/auth/password",
        "/api/auth/email",
    };

    private static readonly string[] EnsureSecurityRoutes =
{
        "/api/user"
    };

    public UserTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IUserService _userService,
        IUserTokenService _tokenService,
        IRoleService _roleService,
        IConfiguration configuration)
    {
        try
        {
            string path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            string method = context.Request.Method;

            if ((method == HttpMethods.Get && !EnsureSecurityRoutes.Any(r => path.StartsWith(r))) || PublicRoutes.Any(r => path.StartsWith(r)))
            {
                await _next(context);
                return;
            }

            string? bearerToken = context.Request.Headers[HeaderNames.Authorization];
            string userAccessToken = bearerToken?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase) ?? "";

            if (string.IsNullOrWhiteSpace(userAccessToken))
                throw new Exception(UserTokenResponse.UserTokenMissing.ToString());

            (Database.Models.User user, string? scope) =
                await _userService.GetUserByTokenAsync(userAccessToken);

            context.Items["CurrentUser"] = user;

        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(ex.Message);
        }
        await _next(context);
    }
}

public static class TokenMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserTokenMiddleware>();
    }
}
