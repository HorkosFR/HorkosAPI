using Microsoft.AspNetCore.Mvc;
using HorkosAPI.Global;
using HorkosAPI.Report.Models;
using HorkosAPI.Report.Services;
using HorkosAPI.User.Models.Enumerations;
using Polly;

namespace HorkosAPI.Report.Controllers
{
    public static class ReportController
    {
        public static WebApplication UseReportController(this WebApplication app)
        {
            var group = app.MapGroup("/api/reports");

            group.MapGet("/", GetReportsAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/{id:Guid}", GetReportByIdAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/{id:Guid}/ban", PostBanReportItem).RequireCors(BaseController.AppOrigin);
            group.MapPut("/{id:Guid}", UpdateReportAsync).RequireCors(BaseController.AppOrigin);
            group.MapPost("/", CreateReportAsync).RequireCors(BaseController.AppOrigin);
            group.MapGet("/new", GetNewReportsAsync).RequireCors(BaseController.AppOrigin);

            return app;
        }

        private static async Task<IResult> GetReportsAsync([FromServices] IReportService service)
        {
            var reports = await service.GetReportsAsync();
            return Results.Ok(reports);
        }

        private static async Task<IResult> GetNewReportsAsync([FromServices] IReportService service)
        {
            var reports = await service.GetNewReportsAsync();
            return Results.Ok(reports);
        }

        private static async Task<IResult> GetReportByIdAsync([FromServices] IReportService service, Guid id)
        {
            var reports = await service.GetReportByIdAsync(id);
            return reports is not null ? Results.Ok(reports) : Results.NotFound();
        }

        private static async Task<IResult> CreateReportAsync([FromServices] IReportService service, [FromBody] ReportDTO report)
        {
            try
            {
                var created = await service.CreateReportAsync(report);
                return Results.Created($"/api/report/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> PostBanReportItem([FromServices] IReportService service, Guid id)
        {
            try
            {
                var created = await service.PostBanReportItem(id);
                return Results.Ok($"Target from ${id} banned");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }

        }

        private static async Task<IResult> UpdateReportAsync(HttpContext context, [FromServices] IReportService service, Guid id, [FromBody] ReportDTO updatedReport)
        {
            try
            {
                context.Items.TryGetValueTyped("CurrentUser", out Database.Models.User currentUser).EnsureTrue(UserResponse.UserDoesNotExist.ToString());
                var success = await service.UpdateReportAsync(id, updatedReport, currentUser.Id);
                return success ? Results.Ok(updatedReport) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, null, 500);
            }
        }
    }
}
