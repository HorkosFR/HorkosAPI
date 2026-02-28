using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Render.Services;

namespace HorkosAPI.Render.Controllers
{
    public static class RenderController
    {
        public static WebApplication UseRenderController(this WebApplication app)
        {
            var group = app.MapGroup("/api/render");

            group.MapGet("/ids", GetIdsAsync);

            return app;
        }

        private static async Task<IResult> GetIdsAsync([FromServices] IRenderService service)
        {
            try
            {
                var ids = await service.GetIdsAsync();
                return Results.Ok(ids);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }
    }
}
