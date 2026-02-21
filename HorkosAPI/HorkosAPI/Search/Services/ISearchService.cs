using HorkosAPI.Search.Models;

namespace HorkosAPI.Search.Services
{
    public interface ISearchService
    {
        Task<List<SearchItem>> GetLatestItemsAsync();
        Task<List<SearchItem>> SearchItemsAsync(string query);
    }
}
