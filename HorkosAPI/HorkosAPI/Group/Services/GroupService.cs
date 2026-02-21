using Microsoft.EntityFrameworkCore;
using Npgsql;
using HorkosAPI.Database.Services;
using HorkosAPI.Group.Models;

namespace HorkosAPI.Group.Services
{
    public class GroupService : IGroupService
    {
        private readonly DatabaseContext _context;

        public GroupService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Database.Models.Group>> GetGroupsAsync() =>
            await _context.Groups
                          .Include(s => s.GroupEntities)
                                .ThenInclude(fcs => fcs.Entity)
                          .ToListAsync();
        public async Task<GetGroupDTO?> GetGroupByIdAsync(Guid id)
        {
            var group = await _context.Groups
                          .Include(g => g.GroupEntities.Where(ge=> ge.Entity.IsVisible == true))
                            .ThenInclude(ge => ge.Entity)
                          .FirstOrDefaultAsync(s => s.Id == id && s.IsVisible == true);
            if (group == null)
            {
                return null;
            }
            return new(group);
        }
    
        public async Task<List<Database.Models.Group>> GetGroupsByEntityIdAsync(Guid entityId) =>
            await _context.Groups
                          .Where(s => s.Id == entityId && s.IsVisible == true)
                          .ToListAsync();

        public async Task<Database.Models.Group> CreateGroupAsync(GroupDTO group, Guid currentUser)
        {
            Database.Models.Group newGroup = new(group);
            newGroup.CreatedBy = currentUser;
            newGroup.LastUpdatedBy = currentUser;
            var existing = await _context.Groups.FirstOrDefaultAsync(g => g.Name == group.Name);
            if (existing != null)
            {
                throw new Exception("Group already exist");
            }
            if (group.EntityId != Guid.Empty)
            {
                var groupEntity = new Database.Models.GroupEntity
                {
                    GroupId = newGroup.Id,
                    EntityId = group.EntityId
                };
                _context.GroupEntities.Add(groupEntity);
            }

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return newGroup;
        }

        public async Task<bool> UpdateGroupAsync(Guid id, GroupDTO updatedGroup, Guid currentUser)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return false;

            group.Name = updatedGroup.Name;
            group.Description = updatedGroup.Description;
            group.ImageUrl = updatedGroup.ImageUrl;
            group.LastUpdatedBy = currentUser;
            group.LastUpdatedAt = DateTime.UtcNow;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LinkGroupEntityAsync(Guid id, GroupLinkDTO body)
        {
            foreach (Guid entityId in body.EntityIds)
            {
                var groupEntity = await _context.GroupEntities.FirstOrDefaultAsync(ge => ge.GroupId == id && ge.EntityId == entityId);
                if (groupEntity != null) continue;

                Database.Models.GroupEntity newGE = new();
                newGE.GroupId = id;
                newGE.EntityId = entityId;

                _context.GroupEntities.Add(newGE);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Database.Models.Group> CreateLinkGroupEntityAsync(Guid entityId, GroupDTO group)
        {
            Database.Models.Group newGroup = new(group);
            var existing = await _context.Groups.FirstOrDefaultAsync(g => g.Name == group.Name);
            if (existing != null)
            {
                throw new Exception("Group already exist");
            }
            var groupEntity = new Database.Models.GroupEntity
            {
                GroupId = newGroup.Id,
                EntityId = group.EntityId
            };
            _context.GroupEntities.Add(groupEntity);

            Database.Models.GroupEntity newGE = new();
            newGE.GroupId = newGroup.Id;
            newGE.EntityId = entityId;

            _context.GroupEntities.Add(newGE);

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return newGroup;
        }

        public async Task<bool> DeleteGroupAsync(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Database.Models.Group>> GetLatestGroupsAsync()
        {
            return await _context.Groups
                                 .OrderByDescending(g => g.Name)
                                 .Take(5)
                                 .ToListAsync();
        }

        public async Task<List<Database.Models.Group>> SearchGroupsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Database.Models.Group>();

            query = query.Trim();

            var parameters = new[] {
                new NpgsqlParameter("@search_exact", $"%{query}%"),
                new NpgsqlParameter("@query", query)
            };

            var groups = await _context.Groups
                .FromSqlRaw(@"
                SET pg_trgm.word_similarity_threshold = 0.3;
                SELECT 
                    *, 
                    word_similarity(""Name"", @query) AS score_proximite
                FROM ""Groups""
                WHERE ""IsVisible"" = true
                  AND (
                      ""Name"" ILIKE @search_exact
                      OR 
                      ""Name"" %> @query
                  )
                ORDER BY ""Name"" ILIKE @search_exact DESC,
                    score_proximite DESC
                LIMIT 10;", parameters)
                .ToListAsync();

            return groups;
        }
    }
}
