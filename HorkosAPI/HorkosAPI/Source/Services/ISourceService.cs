using HorkosAPI.Source.Models;

namespace HorkosAPI.Source.Services
{
    public interface ISourceService
    {
        Task<List<Database.Models.Source>> GetSourcesAsync();
        Task<Database.Models.Source?> GetSourceByIdAsync(Guid id);
        Task<List<Database.Models.Source>> GetSourcesByItemIdAsync(Guid itemId);
        Task<Database.Models.Source> CreateSourceAsync(SourceDTO dto, Guid currentUser);
        Task<bool> UpdateSourceAsync(Guid id, SourceDTO dto, Guid currentUser);
        Task<bool> DeleteSourceAsync(Guid id);
        Task<ItemSourceDTO> GetSourceFromUrlAsync(string url);
    }
}
