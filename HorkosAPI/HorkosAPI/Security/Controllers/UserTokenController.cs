using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Global;
using HorkosAPI.Security.Services;

namespace HorkosAPI.Security.Controllers;

public static class UserTokenController
{
    public static WebApplication UseUserTokenController(this WebApplication app)
    {
        RouteGroupBuilder groupUserToken = app.MapGroup("/api/userToken");
        groupUserToken.MapPost("/CheckRefreshUserToken", CheckRefreshUserTokenAsync).RequireCors(BaseController.AppOrigin); ;
        groupUserToken.MapPost("/CheckAccessUserToken", CheckAccessUserTokenAsync).RequireCors(BaseController.AppOrigin); ;
        groupUserToken.MapGet("/RenewUserToken", RenewUserTokenAsync).RequireCors(BaseController.AppOrigin);
        return app;
    }

    public static async Task<IResult> RenewUserTokenAsync([FromHeader(Name = "authorization")] string refreshUserToken, [FromServices] IUserTokenService _userTokenService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => { return Results.Ok(await _userTokenService.CheckRefreshToken(refreshUserToken)); }
        );

    public static async Task<IResult> CheckRefreshUserTokenAsync([FromHeader(Name = "authorization")] string refreshUserToken, [FromServices] IUserTokenService _userTokenService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => { return Results.Ok(await _userTokenService.CheckRefreshToken(refreshUserToken)); }
        );

    public static async Task<IResult> CheckAccessUserTokenAsync([FromHeader(Name = "authorization")] string accessUserToken, [FromServices] IUserTokenService _userTokenService)
        => await BaseController.ExecuteTryCatchAsync(
            async () => { return Results.Ok(await _userTokenService.CheckAccessToken(accessUserToken)); }
        );
}
