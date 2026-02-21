namespace HorkosAPI.Comment.Controllers
{
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using HorkosAPI.Comment.Models;
    using HorkosAPI.Comment.Services;
    using HorkosAPI.Database.Models;
    using HorkosAPI.Global;
    using HorkosAPI.User.Models.Enumerations;
    using HorkosAPI.UserContribution.Models;
    using HorkosAPI.UserContribution.Services;

    public static class CommentController
    {
        public static WebApplication UseCommentController(this WebApplication app)
        {
            var group = app.MapGroup("/api/comment");

            //group.MapGet("/", GetCommentsAsync);
            group.MapGet("/{id:Guid}", GetCommentByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/target/{targetId:Guid}", GetCommentsByTargetIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateCommentAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateCommentAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteCommentAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetCommentsAsync([FromServices] ICommentService service)
        {
            var comments = await service.GetCommentsAsync();
            return Results.Ok(comments);
        }

        private static async Task<IResult> GetCommentByIdAsync([FromServices] ICommentService service, Guid id)
        {
            var comment = await service.GetCommentByIdAsync(id);
            return comment is not null ? Results.Ok(comment) : Results.NotFound();
        }

        private static async Task<IResult> GetCommentsByTargetIdAsync([FromServices] ICommentService service, Guid targetId)
        {
            var comments = await service.GetCommentsByTargetIdAsync(targetId);
            return Results.Ok(comments);
        }

        private static async Task<IResult> CreateCommentAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ICommentService service, [FromBody] CommentDTO comment)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var created = await service.CreateCommentAsync(comment, currentUser);

                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Comment", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);

                return Results.Created($"/api/comment/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> UpdateCommentAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ICommentService service, Guid id, [FromBody] Comment updatedComment)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            var success = await service.UpdateCommentAsync(id, updatedComment);
            if (success)
            {
                UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "Comment", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
            }

            return success ? Results.Ok(updatedComment) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> DeleteCommentAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] ICommentService service, Guid id)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            if (!currentUser.Role.Name.Equals("Administrator"))
            {
                return Results.Problem("Not enogh rights to delete.", null, 500);
            }
            var success = await service.DeleteCommentAsync(id);

            if (success)
            {
                UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Comment", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);
            }

            return success ? Results.NoContent() : Results.NotFound();
        }
    }
}
