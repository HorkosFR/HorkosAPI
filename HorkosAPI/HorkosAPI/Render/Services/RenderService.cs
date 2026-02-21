using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Services;
using HorkosAPI.Group.Services;

namespace HorkosAPI.Render.Services
{
    public class RenderService : IRenderService
    {
        private readonly DatabaseContext _context;

        public RenderService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetIdsAsync()
        {
            List<string> ids = new List<string>();
            List<Database.Models.Group> groups = await _context.Groups.ToListAsync();
            List<Database.Models.Entity> entities = await _context.Entities.ToListAsync();
            List<Database.Models.Fact> facts = await _context.Facts.ToListAsync();

            groups.ForEach(group => ids.Add("/group/" + group.Id.ToString()));
            entities.ForEach(e => ids.Add("/entity/" + e.Id.ToString()));
            facts.ForEach(f => ids.Add("/entity/" + f.EntityId.ToString() + "/fact/" + f.Id.ToString()));

            return ids;
        }
    }
}
