namespace HorkosAPI.Role.Services
{
    public interface IRoleService
    {
        Task<List<Database.Models.Role>> GetRolesAsync();
        Task<Database.Models.Role?> GetRoleByIdAsync(Guid id);
        Task<Database.Models.Role> CreateRoleAsync(Database.Models.Role Role);
    }
}
