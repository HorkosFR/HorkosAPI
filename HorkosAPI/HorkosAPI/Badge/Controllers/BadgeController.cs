namespace HorkosAPI.Badge.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HorkosAPI.Badge.Services;
    using HorkosAPI.Global;

    public static class BadgeController
    {
        public static WebApplication UseBadgeController(this WebApplication app)
        {
            var group = app.MapGroup("/api/badge");

            group.MapGet("/", GetBadgesAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetBadgesAsync([FromServices] IBadgeService service)
        {
            var badges = await service.GetBadgesAsync();
            return Results.Ok(badges);
        }
    }
}
