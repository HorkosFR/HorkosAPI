using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Database.Models;
using HorkosAPI.FactCheck.Models;
using HorkosAPI.FactCheck.Services;
using HorkosAPI.Global;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;

namespace HorkosAPI.FactCheck.Controllers
{
    public static class FactCheckController
    {
        public static WebApplication UseFactCheckController(this WebApplication app)
        {
            var group = app.MapGroup("/api/factcheck");

            group.MapGet("/", GetFactChecksAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetFactCheckByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/statement/{statementId:Guid}", GetFactChecksByStatementIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateFactCheckAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateFactCheckAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteFactCheckAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetFactChecksAsync([FromServices] IFactCheckService service)
        {
            var factChecks = await service.GetFactChecksAsync();
            return Results.Ok(factChecks);
        }

        private static async Task<IResult> GetFactCheckByIdAsync([FromServices] IFactCheckService service, Guid id)
        {
            var factCheck = await service.GetFactCheckByIdAsync(id);
            return factCheck is not null ? Results.Ok(factCheck) : Results.NotFound();
        }

        private static async Task<IResult> GetFactChecksByStatementIdAsync([FromServices] IFactCheckService service, Guid statementId)
        {
            var factChecks = await service.GetFactChecksByFactIdAsync(statementId);
            return Results.Ok(factChecks);
        }

        private static async Task<IResult> CreateFactCheckAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IFactCheckService service, [FromBody] FactCheckDTO factCheck)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                factCheck.UserId = currentUser.Id;
                var created = await service.CreateFactCheckAsync(factCheck, currentUser.Id);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "FactCheck", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
                return Results.Created($"/api/factcheck/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> UpdateFactCheckAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IFactCheckService service, Guid id, [FromBody] FactCheckDTO updatedFactCheck)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.UpdateFactCheckAsync(id, updatedFactCheck, currentUser.Id);
                if (success)
                {

                    UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "FactCheck", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.Ok(updatedFactCheck) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> DeleteFactCheckAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IFactCheckService service, Guid id)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                if (!currentUser.Role.Name.Equals("Administrator"))
                {
                    return Results.Problem("Not enogh rights to delete.", null, 500);
                }
                var success = await service.DeleteFactCheckAsync(id);
                if (success)
                {

                    UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "FactCheck", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
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
