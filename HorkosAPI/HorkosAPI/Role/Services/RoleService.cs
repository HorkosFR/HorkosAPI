using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Services;

namespace HorkosAPI.Role.Services
{
    public class RoleService : IRoleService
    {
        private readonly DatabaseContext _context;

        public RoleService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Database.Models.Role>> GetRolesAsync() =>
    await _context.Roles
                  .ToListAsync();

        public async Task<Database.Models.Role?> GetRoleByIdAsync(Guid id) =>
            await _context.Roles.AsNoTracking()
                          .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<Database.Models.Role> CreateRoleAsync(Database.Models.Role Role)
        {
            _context.Roles.Add(Role);
            await _context.SaveChangesAsync();
            return Role;
        }
    }
}
