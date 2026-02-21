
namespace HorkosAPI.Badge.Services
{
    public interface IBadgeService
    {
        Task<List<Database.Models.Badge>> GetBadgesAsync();
    }
}
