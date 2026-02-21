using HorkosAPI.FactCheck.Models;

namespace HorkosAPI.FactCheck.Services
{
    public interface IFactCheckService
    {
        Task<List<Database.Models.FactCheck>> GetFactChecksAsync();
        Task<Database.Models.FactCheck?> GetFactCheckByIdAsync(Guid id);
        Task<Database.Models.FactCheck> CreateFactCheckAsync(FactCheckDTO factCheck, Guid currentUser);
        Task<List<Database.Models.FactCheck>> GetFactChecksByFactIdAsync(Guid factId);
        Task<bool> UpdateFactCheckAsync(Guid id, FactCheckDTO updatedFactCheck, Guid currentUser);
        Task<bool> DeleteFactCheckAsync(Guid id);
    }
}
