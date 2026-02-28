namespace HorkosAPI.Global;

public static class BaseController
{
    public const string AppOrigin = "AppOrigin";

    public static IResult NotImplemented() => Results.NotFound();

    public static IResult ExecuteTryCatch(Func<IResult> action, Action<Exception>? actionCatch = null)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            if (actionCatch is not null) actionCatch(e);
            return Results.BadRequest(e.Message);
        }
    }

    public static async Task<IResult> ExecuteTryCatchAsync(Func<Task<IResult>> action, Action<Exception>? actionCatch = null)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            if (actionCatch is not null) actionCatch(e);
            return Results.BadRequest(e.Message);
        }
    }

}
