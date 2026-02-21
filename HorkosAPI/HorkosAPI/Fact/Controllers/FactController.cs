using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Database.Models;
using HorkosAPI.Fact.Models;
using HorkosAPI.Fact.Services;
using HorkosAPI.Global;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;

namespace HorkosAPI.Fact.Controllers
{
    public static class FactController
    {
        public static WebApplication UseFactController(this WebApplication app)
        {
            var group = app.MapGroup("/api/fact");

            group.MapGet("/", GetFactsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/latest", GetLatestFactsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetFactByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/entity/{entityId:Guid}", GetFactsByEntityIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateFactAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/generate", GenerateFactAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateFactAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteFactAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetFactsAsync([FromServices] IFactService service)
        {
            try
            {
                var facts = await service.GetFactsAsync();
                return Results.Ok(facts);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> GetLatestFactsAsync([FromServices] IFactService service)
        {
            try
            {
                var facts = await service.GetLatestFactsAsync();
                return Results.Ok(facts);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> GetFactByIdAsync([FromServices] IFactService service, Guid id)
        {
            try
            {
                var fact = await service.GetFactByIdAsync(id);
                return fact is not null ? Results.Ok(fact) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GetFactsByEntityIdAsync([FromServices] IFactService service, Guid entityId)
        {
            try
            {
                var facts = await service.GetFactsByEntityIdAsync(entityId);
                return Results.Ok(facts);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> CreateFactAsync(HttpContext context, [FromServices] IFactService service, [FromServices] IUserContributionService contribution, [FromBody] FactDTO fact)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await service.CreateFactAsync(fact, currentUser.Id);

                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Fact", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                _ = contribution.CreateContributionAsync(c);
                return Results.Created($"/api/fact/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GenerateFactAsync(HttpContext context, [FromServices] IFactService service, [FromQuery] string url)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var generated = await service.GenerateFactAsync(url);

                return Results.Created($"/api/fact/generated", generated);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> UpdateFactAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IFactService service, Guid id, [FromBody] FactDTO updatedFact)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.UpdateFactAsync(id, updatedFact, currentUser.Id);
                if (success)
                {

                    UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "Fact", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.Ok(updatedFact) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> DeleteFactAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IFactService service, Guid id)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                if (currentUser.Role == null || currentUser.Role.Name == null)
                {
                    return Results.Problem("Not enough rights to delete.", null, 500);
                }
                if (!currentUser.Role.Name.Equals("Administrator"))
                {
                    return Results.Problem("Not enogh rights to delete.", null, 500);
                }
                var success = await service.DeleteFactAsync(id);
                if (success)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Fact", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.NoContent() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }
    }
}
