using HorkosAPI.UserContribution.Models;

namespace HorkosAPI.UserContribution.Services
{
    public interface IUserContributionService
    {
        Task<List<Database.Models.UserContribution>> GetContributionsAsync();
        Task<Database.Models.UserContribution?> GetContributionByIdAsync(Guid id);
        Task<Database.Models.UserContribution> CreateContributionAsync(UserContributionDTO contribution);
        Task<bool> UpdateContributionAsync(Guid id, Database.Models.UserContribution updatedContribution);
        Task<bool> DeleteContributionAsync(Guid id);
    }
}
