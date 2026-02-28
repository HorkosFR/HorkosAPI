using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Email.Models;
using HorkosAPI.Email.Services;
using HorkosAPI.Global;

namespace HorkosAPI.Email.Controller;

public static class EmailController
{
    public static WebApplication UseEmailController(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/email");
        group.MapPost("/first-verification", PostSendFirstVerificationAsync).RequireCors(BaseController.AppOrigin);
        group.MapPost("/verification", PostSendVerificationAsync).RequireCors(BaseController.AppOrigin);
        return app;
    }

    public static async Task<IResult> PostSendFirstVerificationAsync([FromBody] EmailDto emailDto, [FromServices] IEmailService _mailService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _mailService.SendEmailVerificationAsync(emailDto, null))
        );

    public static async Task<IResult> PostSendVerificationAsync([FromBody] EmailDto emailDto, [FromServices] IEmailService _mailService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => Results.Ok(await _mailService.SendEmailVerificationAsync(emailDto, null))
        );
}
