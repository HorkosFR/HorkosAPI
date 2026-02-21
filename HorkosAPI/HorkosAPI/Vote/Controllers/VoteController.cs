using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using HorkosAPI.Database.Models;
using HorkosAPI.Global;
using HorkosAPI.Security.Models.Enumerations;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.User.Services;
using HorkosAPI.UserContribution.Models;
using HorkosAPI.UserContribution.Services;
using HorkosAPI.Vote.Models;
using HorkosAPI.Vote.Services;
using System;
using System.Threading.Tasks;

namespace HorkosAPI.Vote.Controllers
{
    public static class VoteController
    {
        public static WebApplication UseVoteController(this WebApplication app)
        {
            var group = app.MapGroup("/api/vote");

            //group.MapGet("/", GetVotesAsync);
            group.MapGet("/{id:Guid}", GetVoteByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:guid}/me", GetVoteByTargetIdCurrentUserAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:guid}/comment/me", GetVoteByCommentCurrentUserAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateVoteAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateVoteAsync).RequireCors(BaseController.AppOrigin);
            group.MapDelete("/{id:Guid}", DeleteVoteAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetVotesAsync(IVoteService voteService)
        {
            try
            {
                var votes = await voteService.GetVotesAsync();
                return Results.Ok(votes);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> GetVoteByIdAsync(IVoteService voteService, Guid id)
        {
            try
            {
                var vote = await voteService.GetVoteByIdAsync(id);
                return vote is not null ? Results.Ok(vote) : Results.NotFound($"Vote {id} not found");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> GetVoteByTargetIdCurrentUserAsync(HttpContext context, [FromServices] IVoteService voteService, [FromServices] IUserService userService, Guid id)
        {
            try
            {
                string? bearerToken = context.Request.Headers[HeaderNames.Authorization];
                string userAccessToken = bearerToken?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase) ?? "";

                if (string.IsNullOrWhiteSpace(userAccessToken))
                    throw new Exception(UserTokenResponse.UserTokenMissing.ToString());

                (Database.Models.User user, string scope) =
                    await userService.GetUserByTokenAsync(userAccessToken);
                var vote = await voteService.GetVoteByTargetIdCurrentUserAsync(id, user.Id);
                return vote is not null ? Results.Ok(vote) : Results.NotFound($"Vote {id} not found");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> GetVoteByCommentCurrentUserAsync(HttpContext context, [FromServices] IVoteService voteService, [FromServices] IUserService userService, Guid id)
        {
            try
            {
                string? bearerToken = context.Request.Headers[HeaderNames.Authorization];
                string userAccessToken = bearerToken?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase) ?? "";

                if (string.IsNullOrWhiteSpace(userAccessToken))
                    throw new Exception(UserTokenResponse.UserTokenMissing.ToString());

                (Database.Models.User user, string scope) =
                    await userService.GetUserByTokenAsync(userAccessToken);
                var vote = await voteService.GetVoteByCommentCurrentUserAsync(id, user.Id);
                return vote is not null ? Results.Ok(vote) : Results.NotFound($"Vote {id} not found");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> CreateVoteAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IVoteService voteService, VoteDTO vote)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                vote.UserId = currentUser.Id;
                var created = await voteService.CreateVoteAsync(vote);
                UserContributionDTO c = new UserContributionDTO { Action = "Created", ContributionId = created.Id, ContributionType = "Vote", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                _ = contribution.CreateContributionAsync(c);

                return Results.Created($"/api/vote/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> UpdateVoteAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IVoteService voteService, Guid id, VoteDTO vote)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var updated = await voteService.UpdateVoteAsync(id, vote);
                if (updated)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Updated", ContributionId = id, ContributionType = "Vote", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }

                return updated ? Results.Ok($"Vote {id} updated") : Results.NotFound($"Vote {id} not found");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> DeleteVoteAsync(HttpContext context, [FromServices] IUserContributionService contribution, [FromServices] IVoteService voteService, Guid id)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                if (!currentUser.Role.Name.Equals("Administrator"))
                {
                    return Results.Problem("Not enogh rights to delete.", null, 500);
                }
                var deleted = await voteService.DeleteVoteAsync(id);
                if (deleted)
                {
                    UserContributionDTO c = new UserContributionDTO { Action = "Deleted", ContributionId = id, ContributionType = "Vote", UserId = currentUser.Id, Timestamp = DateTime.UtcNow, IpAddress = context.Connection.RemoteIpAddress?.ToString() };
                    _ = contribution.CreateContributionAsync(c);
                }

                return deleted ? Results.Ok($"Vote {id} deleted") : Results.NotFound($"Vote {id} not found");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }
    }
}
