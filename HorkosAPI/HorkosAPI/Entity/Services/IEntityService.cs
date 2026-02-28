using HorkosAPI.Entity.Models;

namespace HorkosAPI.Entity.Services
{
    public interface IEntityService
    {
        Task<List<Database.Models.Entity>> GetEntitiesAsync();
        Task<GetEntityDTO?> GetEntityByIdAsync(Guid id);
        Task<Database.Models.Entity> CreateEntityAsync(EntityDTO entity, Guid currentUser);
        Task<bool> UpdateEntityAsync(Guid id, EntityDTO updatedEntity, Guid currentUser);
        Task<bool> DeleteEntityAsync(Guid id);
        Task<List<Database.Models.Entity>> GetLatestEntitiesAsync();
        Task<List<Database.Models.Entity>> SearchEntitiesAsync(string query);
        Task<Database.Models.Entity> CreateLinkGroupEntityAsync(Guid groupId, EntityDTO entity);
        Task<bool> LinkEntityGroupAsync(Guid id, EntityLinkDTO body);
    }
}
