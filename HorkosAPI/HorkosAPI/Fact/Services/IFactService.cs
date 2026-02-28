using HorkosAPI.Fact.Models;

namespace HorkosAPI.Fact.Services
{
    public interface IFactService
    {
        Task<List<Database.Models.Fact>> GetFactsAsync();
        Task<Database.Models.Fact?> GetFactByIdAsync(Guid id);
        Task<Database.Models.Fact> CreateFactAsync(FactDTO fact, Guid userId);
        Task<bool> UpdateFactAsync(Guid id, FactDTO updatedFact, Guid currentUser);
        Task<bool> DeleteFactAsync(Guid id);
        Task<List<Database.Models.Fact>> GetFactsByEntityIdAsync(Guid entityId);
        Task<List<HomeFact>> GetLatestFactsAsync();
        Task<FactDTO> GenerateFactAsync(string url);
    }
}
