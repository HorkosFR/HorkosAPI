using HorkosAPI.Global;

namespace HorkosAPI.UserContribution.Controllers
{
    public static class UserContributionController
    {
        public static WebApplication UseUserContributionController(this WebApplication app)
        {
            var group = app.MapGroup("/api/contribution");

            //group.MapGet("/", GetContributionsAsync);
            group.MapGet("/{id:Guid}", GetContributionByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateContributionAsync).RequireCors(BaseController.AppOrigin);
            //group.MapDelete("/{id:Guid}", DeleteContributionAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static IResult GetContributionsAsync() => Results.Ok("Contributions list");
        private static IResult GetContributionByIdAsync(Guid id) => Results.Ok($"Contribution {id}");
        private static IResult CreateContributionAsync() => Results.Ok("Contribution created");
        private static IResult DeleteContributionAsync(Guid id) => Results.Ok($"Contribution {id} deleted");

    }
}
