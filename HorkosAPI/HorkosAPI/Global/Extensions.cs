using Newtonsoft.Json.Linq;
namespace HorkosAPI.Global;

public static class Extensions
{
    public static T EnsureNotNull<T>(this T? obj, string message) => obj == null ? throw new Exception(message) : obj;

    public static async Task<T> EnsureResultNotNull<T>(this Task<T?> task, string message)
    {
        T? result = await task;
        return result == null ? throw new Exception(message) : result;
    }


    public static async Task EnsureResultNull<T>(this Task<T?> task, string message)
    {
        T? result = await task;
        if (result is not null)
        {
            throw new Exception(message);
        }
    }

    public static bool TryGetValueTyped<T>(this IDictionary<object, object?> dict, object key, out T value)
    {
        if (dict.TryGetValue(key, out object valueResult))
        {
            value = (T)valueResult;
            return true;
        }

        value = default;
        return false;
    }


    public static void EnsureTrue(this bool boolEnsure, string errorMessage)
    {
        if (!boolEnsure)
        {
            throw new Exception(errorMessage);
        }
    }
}