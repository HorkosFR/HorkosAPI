using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Database.Models;
using HorkosAPI.Entity.Models;
using HorkosAPI.Entity.Services;
using HorkosAPI.Global;
using HorkosAPI.Group.Models;
using HorkosAPI.Group.Services;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;
using System.Text.RegularExpressions;

namespace HorkosAPI.Entity.Controllers
{
    public static class EntityController
    {
        public static WebApplication UseEntityController(this WebApplication app)
        {
            var group = app.MapGroup("/api/entity");

            group.MapGet("/", GetEntitiesAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/latest", GetLatestEntitiesAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/search", SearchEntitiesAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetEntityByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateEntityAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateEntityAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteEntityAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/{id:guid}/link", LinkEntityGroupAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/group/{groupId:guid}/createlink", CreateLinkGroupEntityAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetEntitiesAsync([FromServices] IEntityService _entityService)
            => Results.Ok(await _entityService.GetEntitiesAsync());

        private static async Task<IResult> GetEntityByIdAsync([FromServices] IEntityService _entityService, Guid id)
            => Results.Ok(await _entityService.GetEntityByIdAsync(id));

        private static async Task<IResult> CreateEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IEntityService _entityService, [FromBody] EntityDTO entity)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await _entityService.CreateEntityAsync(entity, currentUser.Id);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Entity", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                _ = contribution.CreateContributionAsync(c);
                return Results.Ok(created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> CreateLinkGroupEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IEntityService _entityService, Guid groupId, [FromBody] EntityDTO entity)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await _entityService.CreateLinkGroupEntityAsync(groupId, entity);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Entity", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                _ = contribution.CreateContributionAsync(c);
                return Results.Ok(created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> UpdateEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IEntityService _entityService, [FromBody] EntityDTO entity, Guid id)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await _entityService.UpdateEntityAsync(id, entity, currentUser.Id);
                if (success)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = id, ContributionType = "Entity", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.Ok(entity) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> DeleteEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IEntityService _entityService, Guid id)
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
                    return Results.Problem("Not enough rights to delete.", null, 500);
                }
                var success = await _entityService.DeleteEntityAsync(id);
                if (success)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Entity", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.NoContent() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GetLatestEntitiesAsync([FromServices] IEntityService _entityService)
        {
            try
            {
                return Results.Ok(await _entityService.GetLatestEntitiesAsync());
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> SearchEntitiesAsync([FromServices] IEntityService _entityService, [FromQuery] string query)
        {
            try
            {
                return Results.Ok(await _entityService.SearchEntitiesAsync(query));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> LinkEntityGroupAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IEntityService service, Guid id, [FromBody] EntityLinkDTO groupIds)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.LinkEntityGroupAsync(id, groupIds);
                if (success)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Linked", ContributionId = id, ContributionType = "Entity", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "" };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.Ok() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }
    }
}
