using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Auth.Models;
using HorkosAPI.Auth.Services;
using HorkosAPI.Email.Models;
using HorkosAPI.Email.Services;
using HorkosAPI.Global;
using HorkosAPI.User.Models;
using HorkosAPI.User.Services;

namespace HorkosAPI.Auth.Controllers;

public static class AuthController
{
    public static WebApplication UseAuthController(this WebApplication app)
    {
        RouteGroupBuilder groupAuth = app.MapGroup("/api/auth");
        groupAuth.MapPost("/register", PostRegisterAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPost("/resend", PostResendAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPost("/login", PostLoginAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPost("/verify", PostVerifyAccountAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPost("/update-password", PostUpdatePasswordAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPatch("/password", PatchUpdatePasswordAsync).RequireCors(BaseController.AppOrigin);
        groupAuth.MapPatch("/email", PatchUpdateEmailAsync).RequireCors(BaseController.AppOrigin);

        return app;
    }

    public static async Task<IResult> PostRegisterAsync([FromBody] RegisterDto registerDto, [FromServices] IAuthService _authService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _authService.RegisterUserAsync(registerDto)),
            (e) => Results.BadRequest(e)
        );

    public static async Task<IResult> PostResendAsync([FromBody] RegisterDto registerDto, [FromServices] IAuthService _authService)
    => await BaseController.ExecuteTryCatchAsync(
        async () => Results.Ok(await _authService.ResendEmailAsync(registerDto)),
        (e) => Results.BadRequest(e)
    );

    public static async Task<IResult> PostLoginAsync([FromBody] LoginDto loginDto, [FromServices] IAuthService _authService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _authService.LoginUserAsync(loginDto)),
            (e) => Results.BadRequest(e)
        );

    public static async Task<IResult> PostVerifyAccountAsync([FromBody] VerificationDto verificationDto, [FromServices] IAuthService _authService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _authService.VerifyAccountAsync(verificationDto)),
            (e) => Results.BadRequest(e)
        );

    public static async Task<IResult> PostUpdatePasswordAsync([FromBody] ForgotPasswordDto emailDto, [FromServices] IEmailService _mailService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _mailService.SendEmailPasswordAsync(emailDto, null))
        );


    public static async Task<IResult> PatchUpdatePasswordAsync(HttpContext context, [FromBody] PasswordDto passwordDto, [FromServices] IUserService _userService)
        => await BaseController.ExecuteTryCatchAsync(
            async () =>
            {
                return Results.Ok(await _userService.UpdatePasswordAsync(passwordDto));
            }
        );

    public static async Task<IResult> PatchUpdateEmailAsync(HttpContext context, [FromBody] UpdateEmailDto emailToken, [FromServices] IUserService _userService)
        => await BaseController.ExecuteTryCatchAsync(
            async () =>
            {
                return Results.Ok(await _userService.UpdateEmailAsync(emailToken));
            }
        );
}
