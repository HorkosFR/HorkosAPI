namespace HorkosAPI.Render.Services
{
    public interface IRenderService
    {
        Task<List<string>> GetIdsAsync();
    }
}
