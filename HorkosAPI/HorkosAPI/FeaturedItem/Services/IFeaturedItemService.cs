namespace HorkosAPI.FeaturedItem.Services
{
    public interface IFeaturedItemService
    {
        Task<List<Database.Models.FeaturedItem>> GetFeaturedItemAsync();
    }
}
