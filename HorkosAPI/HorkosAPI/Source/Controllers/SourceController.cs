using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Database.Models;
using HorkosAPI.Fact.Services;
using HorkosAPI.Global;
using HorkosAPI.Source.Models;
using HorkosAPI.Source.Services;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;

namespace HorkosAPI.Source.Controllers
{
    public static class SourceController
    {
        public static WebApplication UseSourceController(this WebApplication app)
        {
            var group = app.MapGroup("/api/source");

            group.MapGet("/", GetSourcesAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetSourceByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/item/{itemId:Guid}", GetSourcesByItemIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateSourceAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateSourceAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteSourceAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/generate", GetSourceFromUrlAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetSourcesAsync([FromServices] ISourceService service)
        {
            var sources = await service.GetSourcesAsync();
            return Results.Ok(sources);
        }

        private static async Task<IResult> GetSourceByIdAsync([FromServices] ISourceService service, Guid id)
        {
            var source = await service.GetSourceByIdAsync(id);
            return source is not null ? Results.Ok(source) : Results.NotFound();
        }

        private static async Task<IResult> GetSourcesByItemIdAsync([FromServices] ISourceService service, Guid itemId)
        {
            var sources = await service.GetSourcesByItemIdAsync(itemId);
            return Results.Ok(sources);
        }

        private static async Task<IResult> GetSourceFromUrlAsync(HttpContext context, [FromServices] ISourceService service, [FromQuery] string url)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var generated = await service.GetSourceFromUrlAsync(url);

                return Results.Created($"/api/fact/generated", generated);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> CreateSourceAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ISourceService service, [FromBody] SourceDTO dto)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            var created = await service.CreateSourceAsync(dto, currentUser.Id);
            UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Source", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
            _ = contribution.CreateContributionAsync(c);
            return Results.Created($"/api/source/{created.Id}", created);
        }

        private static async Task<IResult> UpdateSourceAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ISourceService service, Guid id, [FromBody] SourceDTO dto)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            var success = await service.UpdateSourceAsync(id, dto, currentUser.Id);
            if (success)
            {

                UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "Source", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
            }
            return success ? Results.Ok() : Results.NotFound();
        }

        private static async Task<IResult> DeleteSourceAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ISourceService service, Guid id)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            if (!currentUser.Role.Name.Equals("Administrator"))
            {
                return Results.Problem("Not enogh rights to delete.", null, 500);
            }
            var success = await service.DeleteSourceAsync(id);
            if (success)
            {

                UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Source", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
            }
            return success ? Results.NoContent() : Results.NotFound();
        }
    }
}
