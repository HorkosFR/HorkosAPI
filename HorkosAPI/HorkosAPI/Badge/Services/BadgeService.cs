namespace HorkosAPI.Badge.Services
{
    using Microsoft.EntityFrameworkCore;
    using HorkosAPI.Database.Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BadgeService : IBadgeService
    {
        private readonly DatabaseContext _context;

        public BadgeService(DatabaseContext context)
        {
            _context = context;
        }

        /*
        public async Task<List<Database.Models.Badge>> GetBadgesAsync()
        {
            return _json.Badges.ToList();

        }
        */

        
        public async Task<List<Database.Models.Badge>> GetBadgesAsync() =>
            await _context.Badges
                          .ToListAsync();
        
    }
}
