using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Global;
using HorkosAPI.User.Models;
using HorkosAPI.User.Models.Enumerations;
using HorkosAPI.User.Services;
using Polly;

namespace HorkosAPI.User.Controllers
{
    public static class UserController
    {
        public static WebApplication UseUserController(this WebApplication app)
        {
            var group = app.MapGroup("/api/user");

            //group.MapGet("/", GetUsersAsync);
            group.MapGet("/{id:Guid}", GetUserByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateUserAsync).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateUserAsync).RequireCors(BaseController.AppOrigin);
            //group.MapDelete("/{id:Guid}", DeleteUserAsync);

            group.MapGet("/me", GetCurrentUserAsync);
            group.MapPut("/me", UpdateCurrentUserAsync);

            return app;
        }

        /*
        private static async Task<IResult> GetUsersAsync([FromServices] IUserService userService)
            => await BaseController.ExecuteTryCatchAsync(
                async () => Results.Ok(await userService.GetUsersAsync()),
                (e) => loggerFactory.CreateLogger("UserController").LogError(e, "Error fetching users.")
            );
        */
        private static async Task<IResult> GetUserByIdAsync(HttpContext context, Guid id, [FromServices] IUserService userService)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            if (currentUser.Id != id)
            {
                return Results.Problem(UserResponse.UserDoesNotExist.ToString(), null, 500);
            }
            return await BaseController.ExecuteTryCatchAsync(
                async () => Results.Ok(await userService.GetUserByIdAsync(id))
            );

        }

        private static async Task<IResult> CreateUserAsync([FromBody] Database.Models.User user, [FromServices] IUserService userService)
            => await BaseController.ExecuteTryCatchAsync(
                async () => Results.Ok(await userService.CreateUserAsync(user))
            );

        private static async Task<IResult> UpdateUserAsync(HttpContext context, Guid id, [FromBody] Database.Models.User updatedUser, [FromServices] IUserService userService)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            if (currentUser.Id != id)
            {
                return Results.Problem(UserResponse.UserDoesNotExist.ToString(), null, 500);
            }
            return await BaseController.ExecuteTryCatchAsync(
                async () => Results.Ok(await userService.UpdateUserAsync(id, updatedUser))
            );
        }

        private static async Task<IResult> DeleteUserAsync(Guid id, [FromServices] IUserService userService)
            => await BaseController.ExecuteTryCatchAsync(
                async () => Results.Ok(await userService.DeleteUserAsync(id))
            );

        private static async Task<IResult> GetCurrentUserAsync(
            HttpContext context,
            [FromServices] IUserService userService)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            return await BaseController.ExecuteTryCatchAsync(
                    async () => Results.Ok(userService.GetCurrentUserAsync(currentUser))
                );
        }
        private static async Task<IResult> UpdateCurrentUserAsync(
            HttpContext context,
            [FromBody] UpdateUserDTO updateDto,
            [FromServices] IUserService userService)
        {
            context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
            return await BaseController.ExecuteTryCatchAsync(
                    async () => Results.Ok(await userService.UpdateCurrentUserAsync(currentUser, updateDto))
                );

        }
    }
}
