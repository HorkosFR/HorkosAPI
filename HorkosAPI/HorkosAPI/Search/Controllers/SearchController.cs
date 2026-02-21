using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Global;
using HorkosAPI.Group.Services;
using HorkosAPI.Search.Services;

namespace HorkosAPI.Search.Controllers
{
    public static class SearchController
    {
        public static WebApplication UseSearchController(this WebApplication app)
        {
            var group = app.MapGroup("/api/search");


            group.MapGet("/latest", GetLatestItemsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/", SearchItemsAsync).RequireCors(BaseController.AppOrigin);
            return app;
        }

        private static async Task<IResult> GetLatestItemsAsync([FromServices] ISearchService _searchService)
        {
            try
            {
                return Results.Ok(await _searchService.GetLatestItemsAsync());
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }

        private static async Task<IResult> SearchItemsAsync([FromServices] ISearchService _searchService, [FromQuery] string query)
        {
            try
            {
                return Results.Ok(await _searchService.SearchItemsAsync(query));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }
    }
}
