using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Entity.Services;
using HorkosAPI.Global;
using HorkosAPI.Group.Models;
using HorkosAPI.Group.Services;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;

namespace HorkosAPI.Group.Controllers
{
    public static class GroupController
    {
        public static WebApplication UseGroupController(this WebApplication app)
        {
            var group = app.MapGroup("/api/group");

            group.MapGet("/", GetGroupsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetGroupByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/latest", GetLatestGroupsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/search", SearchGroupsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/entity/{entityId:Guid}", GetGroupsByEntityIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateGroupAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateGroupAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteGroupAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/{id:guid}/link", LinkGroupEntityAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/entity/{entityId:guid}/createlink", CreateLinkGroupEntityAsync).RequireCors(BaseController.AppOrigin);
            return app;
        }

        private static async Task<IResult> GetGroupsAsync([FromServices] IGroupService service)
        {
            try
            {
                var groups = await service.GetGroupsAsync();
                return Results.Ok(groups);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GetGroupByIdAsync([FromServices] IGroupService service, Guid id)
        {
            try
            {
                var group = await service.GetGroupByIdAsync(id);
                return group is not null ? Results.Ok(group) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GetGroupsByEntityIdAsync([FromServices] IGroupService service, Guid entityId)
        {
            try
            {
                var groups = await service.GetGroupsByEntityIdAsync(entityId);
                return Results.Ok(groups);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> CreateGroupAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IGroupService service, [FromBody] GroupDTO group)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await service.CreateGroupAsync(group, currentUser.Id);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Group", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
                return Results.Created($"/api/group/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> UpdateGroupAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IGroupService service, Guid id, [FromBody] GroupDTO updatedGroup)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.UpdateGroupAsync(id, updatedGroup, currentUser.Id);
                if (success)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "Group", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.Ok(updatedGroup) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> LinkGroupEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IGroupService service, Guid id, [FromBody] GroupLinkDTO entityIds)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.LinkGroupEntityAsync(id, entityIds);
                if (success)
                {

                    UserContributionDTO c = new UserContributionDTO { Action = "Linked", ContributionId = id, ContributionType = "Group", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }

                return success ? Results.Ok() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> CreateLinkGroupEntityAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IGroupService service, Guid entityId, [FromBody] GroupDTO group)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await service.CreateLinkGroupEntityAsync(entityId, group);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Group", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
                return Results.Created($"/api/group/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> DeleteGroupAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IGroupService service, Guid id)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                if (!currentUser.Role.Name.Equals("Administrator"))
                {
                    return Results.Problem("Not enogh rights to delete.", null, 500);
                }
                var success = await service.DeleteGroupAsync(id);
                if (success)
                {

                    UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Group", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }
                return success ? Results.NoContent() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> GetLatestGroupsAsync([FromServices] IGroupService _groupService)
        {
            try
            {
                return Results.Ok(await _groupService.GetLatestGroupsAsync());
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> SearchGroupsAsync([FromServices] IGroupService _groupService, [FromQuery] string query)
        {
            try
            {
                return Results.Ok(await _groupService.SearchGroupsAsync(query));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

    }
}
