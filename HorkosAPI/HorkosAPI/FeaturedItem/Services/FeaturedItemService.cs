using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Search.Models;

namespace HorkosAPI.FeaturedItem.Services
{
    public class FeaturedItemService : IFeaturedItemService
    {
        private readonly DatabaseContext _context;

        public FeaturedItemService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Database.Models.FeaturedItem>> GetFeaturedItemAsync()
        {
            var now = DateTime.UtcNow;

            List<Database.Models.FeaturedItem> items = await _context.FeaturedItems.Where(f => f.StartDate <= now && f.EndDate >= now).OrderByDescending(f => f.Priority).Take(6).ToListAsync();
            var groupIds = items
                .Where(f => f.ItemType == "group")
                .Select(f => f.ItemId)
                .ToList();

            var entityIds = items
                .Where(f => f.ItemType == "entity")
                .Select(f => f.ItemId)
                .ToList();

            List<Database.Models.Group> groups = await _context.Groups
                                                     .Where(g => groupIds.Contains(g.Id))
                                                     .ToListAsync();

            List<Database.Models.Entity> entities = await _context.Entities
                                         .Where(e => entityIds.Contains(e.Id))
                                         .ToListAsync();

            var groupMap = groups.ToDictionary(g => g.Id);
            var entityMap = entities.ToDictionary(e => e.Id);

            foreach(var f in items)
            {
                if (f.ItemType == "group" && groupMap.TryGetValue(f.ItemId, out var group))
                   f.Item = new(group);
                if (f.ItemType == "entity" && entityMap.TryGetValue(f.ItemId, out var entity))
                    f.Item = new(entity);
            }

            return items;
        }
    }
}
