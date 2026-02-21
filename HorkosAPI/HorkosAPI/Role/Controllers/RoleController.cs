using HorkosAPI.Global;
using HorkosAPI.Role.Services;

namespace HorkosAPI.Role.Controllers
{
    public static class RoleController
    {
        public static WebApplication UseRoleController(this WebApplication app)
        {
            var group = app.MapGroup("/api/role");

            group.MapGet("/", GetRolesAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetRoleByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateRoleAsync).RequireCors(BaseController.AppOrigin);
            //group.MapDelete("/{id:Guid}", DeleteRoleAsync);

            return app;
        }

        private static IResult GetRolesAsync() => Results.Ok("Roles list");
        private static IResult GetRoleByIdAsync(Guid id) => Results.Ok($"Role {id}");
        private static async Task<IResult> CreateRoleAsync(IRoleService roleService, Database.Models.Role role)
        {
            var createdRole = await roleService.CreateRoleAsync(role);
            return Results.Created($"/api/role/{createdRole.Id}", createdRole);
        }
        private static IResult DeleteRoleAsync(Guid id) => Results.Ok();

    }
}
