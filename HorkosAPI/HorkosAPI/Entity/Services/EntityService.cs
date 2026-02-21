using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Entity.Models;
using HorkosAPI.Group.Models;
using System.Web.Helpers;

namespace HorkosAPI.Entity.Services;
public class EntityService : IEntityService
{
    
    private readonly DatabaseContext _context;

    public EntityService(DatabaseContext context)
    {
        _context = context;
    }
    public async Task<List<Database.Models.Entity>> GetEntitiesAsync() =>
        await _context.Entities
                      .Include(e => e.FactEntities)
                      .ToListAsync();

    public async Task<GetEntityDTO?> GetEntityByIdAsync(Guid id)
    {
        var entity =  await _context.Entities
            .Include(e => e.FactEntities.Where(fe=> fe.Fact.IsVisible == true).OrderByDescending(fe => fe.Fact.StartDate)
        .ThenByDescending(fe => fe.Fact.CreatedAt))
                .ThenInclude(ps => ps.Fact)
                    .ThenInclude(f => f.FactChecks.Where(fc => fc.IsVisible == true))
                        .ThenInclude(fc => fc.FactCheckSources.Where(fcs => fcs.Source.IsVisible == true))
                            .ThenInclude(fcs => fcs.Source)
            .Include(e => e.FactEntities)
                .ThenInclude(ps => ps.Fact)
                        .ThenInclude(f => f.FactSources.Where(fs => fs.Source.IsVisible == true))
                            .ThenInclude(fs => fs.Source)
            .Include(e => e.GroupEntities.Where(ge => ge.Group.IsVisible == true))
                .ThenInclude(ge => ge.Group)

            .FirstOrDefaultAsync(e => e.Id == id && e.IsVisible);
        if (entity == null)
            return null;
        return new(entity);
    }
    public async Task<List<Database.Models.Entity>> GetLatestEntitiesAsync()
    {
        return await _context.Entities
                             .OrderByDescending(e => e.CreatedAt)
                             .Where(e => e.IsVisible)
                             .Take(5)
                             .ToListAsync();
    }

    public async Task<List<Database.Models.Entity>> SearchEntitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<Database.Models.Entity>();

        query = query.Trim();

        var parameters = new[] {
                new NpgsqlParameter("@search_exact", $"%{query}%"),
                new NpgsqlParameter("@query", query)
            };

        var entities = await _context.Entities
            .FromSqlRaw(@"
                SET pg_trgm.word_similarity_threshold = 0.3;
                SELECT 
                    *, 
                    word_similarity(""Name"", @query) AS score_proximite
                FROM ""Entities""
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

        return entities;
    }


    public async Task<bool> LinkEntityGroupAsync(Guid id, EntityLinkDTO body)
    {
        foreach (Guid groupId in body.GroupIds)
        {
            var groupEntity = await _context.GroupEntities.FirstOrDefaultAsync(ge => ge.EntityId == id && ge.GroupId == groupId);
            if (groupEntity != null) continue ;

            Database.Models.GroupEntity newGE = new();
            newGE.GroupId = groupId;
            newGE.EntityId = id;

            _context.GroupEntities.Add(newGE);
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Database.Models.Entity> CreateEntityAsync(EntityDTO entity, Guid currentUser)
    {
        Database.Models.Entity newEntity = new Database.Models.Entity(entity);
        newEntity.CreatedBy = currentUser;
        newEntity.LastUpdatedBy = currentUser;
        var existing = await _context.Entities.FirstOrDefaultAsync(e => e.Name == entity.Name);
        if (existing != null)
        {
            throw new Exception("Entity already exist");
        }

        if (entity.GroupId != null && entity.GroupId.Count > 0)
        {
            foreach (Guid g in entity.GroupId)
            {
                var groupEntity = new Database.Models.GroupEntity
                {
                    GroupId = g,
                    EntityId = newEntity.Id
                };
                _context.GroupEntities.Add(groupEntity);
            }
        }

        _context.Entities.Add(newEntity);
        await _context.SaveChangesAsync();
        return newEntity;
    }

    public async Task<Database.Models.Entity> CreateLinkGroupEntityAsync(Guid groupId, EntityDTO entity)
    {
        Database.Models.Entity newEntity = new Database.Models.Entity(entity);
        var existing = await _context.Entities.FirstOrDefaultAsync(e => e.Name == entity.Name);
        if (existing != null)
        {
            throw new Exception("Entity already exist");
        }

        _context.Entities.Add(newEntity);

        Database.Models.GroupEntity newGE = new();
        newGE.GroupId = newEntity.Id;
        newGE.EntityId = groupId;

        _context.GroupEntities.Add(newGE);

        await _context.SaveChangesAsync();
        return newEntity;
    }

    public async Task<bool> UpdateEntityAsync(Guid id, EntityDTO updatedEntity, Guid currentUser)
    {
        var entity = await _context.Entities.FindAsync(id);
        if (entity == null) return false;

        entity.Name = updatedEntity.Name;
        entity.Type = updatedEntity.Type;
        entity.ShortBio = updatedEntity.ShortBio;
        entity.Sector = updatedEntity.Sector;
        entity.Country = updatedEntity.Country;
        entity.BirthDate = updatedEntity.BirthDate;
        entity.FoundedDate = updatedEntity.FoundedDate;
        entity.ImageUrl = updatedEntity.ImageUrl;
        entity.OfficialLinks = updatedEntity.OfficialLinks == null ? "" : string.Join(';', updatedEntity.OfficialLinks);
        entity.LastUpdatedAt = DateTime.UtcNow;
        entity.LastUpdatedBy = currentUser;

        _context.Entities.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEntityAsync(Guid id)
    {
        var entity = await _context.Entities.FindAsync(id);
        if (entity == null) return false;

        _context.Entities.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
