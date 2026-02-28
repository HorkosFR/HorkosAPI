namespace HorkosAPI.Global;
public static class DisponibilityController
{
    public static WebApplication UseDisponibilityController(this WebApplication app)
    {
        RouteGroupBuilder groupDisponibility = app.MapGroup("/");
        groupDisponibility.MapGet("/disponibility", Disponibility).RequireCors(BaseController.AppOrigin);
        groupDisponibility.MapMethods("/disponibility", new[] { "HEAD" }, Disponibility).RequireCors(BaseController.AppOrigin);
        return app;
    }

    public static IResult Disponibility()
    {
        return Results.Ok();
    }
}
