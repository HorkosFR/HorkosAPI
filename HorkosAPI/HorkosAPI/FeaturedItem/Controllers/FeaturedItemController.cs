using Microsoft.AspNetCore.Mvc;
using HorkosAPI.FeaturedItem.Services;
using HorkosAPI.Global;
using HorkosAPI.Search.Services;

namespace HorkosAPI.FeaturedItem.Controllers
{
    public static class FeaturedItemController
    {
        public static WebApplication UseFeaturedItemController(this WebApplication app)
        {
            var group = app.MapGroup("/api/featured-item");


            group.MapGet("/", GetFeaturedItem).RequireCors(BaseController.AppOrigin);
            return app;
        }

        private static async Task<IResult> GetFeaturedItem([FromServices] IFeaturedItemService _featuredItemService)
        {
            try
            {
                return Results.Ok(await _featuredItemService.GetFeaturedItemAsync());
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }
    }
}
