using HorkosAPI.Fact.Models;
using HorkosAPI.Group.Models;

namespace HorkosAPI.Group.Services
{
    public interface IGroupService
    {
        Task<List<Database.Models.Group>> GetGroupsAsync();
        Task<GetGroupDTO?> GetGroupByIdAsync(Guid id);
        Task<Database.Models.Group> CreateGroupAsync(GroupDTO fact, Guid currentUser);
        Task<bool> UpdateGroupAsync(Guid id, GroupDTO updatedGroup, Guid currentUser);
        Task<bool> DeleteGroupAsync(Guid id);
        Task<List<Database.Models.Group>> GetGroupsByEntityIdAsync(Guid entityId);
        Task<List<Database.Models.Group>> GetLatestGroupsAsync();
        Task<List<Database.Models.Group>> SearchGroupsAsync(string query);
        Task<bool> LinkGroupEntityAsync(Guid id, GroupLinkDTO body);
        Task<Database.Models.Group> CreateLinkGroupEntityAsync(Guid entityId, GroupDTO group);
    }
}
