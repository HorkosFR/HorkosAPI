using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Fact.Models;
using HorkosAPI.Gamification.Services;
using HorkosAPI.Source.Models;
using SmartReader;
using System.Web.Helpers;


namespace HorkosAPI.Fact.Services
{
    public class FactService : IFactService
    {
        private readonly DatabaseContext _context;
        private readonly GamificationService _gamificationService;

        public FactService(DatabaseContext context)
        {
            _context = context;
            _gamificationService = new GamificationService(context);
        }

        public async Task<List<Database.Models.Fact>> GetFactsAsync() =>
            await _context.Facts
                          .Include(s => s.FactSources)
                            .ThenInclude(fs => fs.Source)
                          .Include(s => s.FactChecks)
                            .ThenInclude(fc => fc.FactCheckSources)
                                .ThenInclude(fcs => fcs.Source)
                          .ToListAsync();

        public async Task<Database.Models.Fact?> GetFactByIdAsync(Guid id) =>
            await _context.Facts
                          .Include(s => s.FactSources.Where(fs => fs.Source.IsVisible == true))
                            .ThenInclude(fs => fs.Source)
                          .Include(s => s.FactChecks.Where(fc => fc.IsVisible == true))
                            .ThenInclude(fc => fc.FactCheckSources.Where(fcs => fcs.Source.IsVisible == true))
                                .ThenInclude(fcs => fcs.Source)
                          .Include(s => s.FactChecks.Where(fc => fc.IsVisible == true))
                                .ThenInclude(fc => fc.User)
                          .FirstOrDefaultAsync(s => s.Id == id);
        public async Task<List<Database.Models.Fact>> GetFactsByEntityIdAsync(Guid entityId) =>
            await _context.Facts
                          .Include(s => s.FactSources.Where(fs => fs.Source.IsVisible == true))
                            .ThenInclude(fs => fs.Source)
                          .Include(s => s.FactChecks.Where(fc => fc.IsVisible == true))
                            .ThenInclude(fc => fc.FactCheckSources.Where(fcs => fcs.Source.IsVisible == true))
                                .ThenInclude(fcs => fcs.Source)
                          .Where(s => s.EntityId == entityId)
                          .ToListAsync();

        public async Task<List<HomeFact>> GetLatestFactsAsync()
        {
            List<HomeFact> homeFacts = new();
            List<Database.Models.Fact> facts = await _context.Facts.Include(f => f.Entity).Where(f => f.IsVisible && f.Entity.IsVisible).OrderByDescending(f => f.CreatedAt).Take(6).ToListAsync();
            foreach (Database.Models.Fact f in facts)
            {
                HomeFact hf = new(f);
                homeFacts.Add(hf);
            }
            return homeFacts;
        }

        public async Task<Database.Models.Fact> CreateFactAsync(FactDTO fact, Guid userId)
        {
            Database.Models.Fact newFact = new(fact);
            newFact.CreatedBy = userId;
            newFact.LastUpdatedBy = userId;
            if (fact.Source != null)
            {
                foreach (ItemSourceDTO i in fact.Source)
                {
                    Database.Models.Source source = new(i);
                    source.CreatedBy = userId;
                    source.LastUpdatedBy = userId;
                    _context.Sources.Add(source);
                    var factSource = new Database.Models.FactSource
                    {
                        FactId = newFact.Id,
                        SourceId = source.Id
                    };
                    _context.FactSources.Add(factSource);
                }

            }

            var factEntity = new Database.Models.FactEntity
            {
                FactId = newFact.Id,
                EntityId = fact.EntityId
            };
            _context.FactEntities.Add(factEntity);

            _context.Facts.Add(newFact);
            await _context.SaveChangesAsync();

            if (userId != Guid.Empty)
                await _gamificationService.AwardPointsAsync(userId, "FACT_CREATED", newFact.Id);

            return newFact;
        }

        public async Task<FactDTO> GenerateFactAsync(string url)
        {
            FactDTO newFact = new();
            newFact.Source = new();
            ItemSourceDTO newSource = new();
            if (string.IsNullOrEmpty(url)) throw new Exception("Missing URL.");

            var reader = new Reader(url);
            var article = await reader.GetArticleAsync();

            if (!article.IsReadable)
                throw new Exception("Wrong URL.");

            newFact.Title = article.Title;
            newSource.Author = article.Byline;
            newFact.Type = "PublicStatement";
            newSource.Url = url;
            newFact.Summary = article.Excerpt;
            newFact.Content = article.TextContent;
            newFact.StartDate = DateTime.UtcNow;
            newSource.Title = article.Title;
            newSource.PublicationDate = article.PublicationDate ?? DateTime.UtcNow;
            newSource.SourceType = "Article";
            newFact.Source.Add(newSource);
            return newFact;
        }

        public async Task<bool> UpdateFactAsync(Guid id, FactDTO updatedFact, Guid currentUser)
        {
            var fact = await _context.Facts.FindAsync(id);
            if (fact == null) return false;

            fact.Title = updatedFact.Title;
            fact.Type = updatedFact.Type;
            fact.StartDate = updatedFact.StartDate;
            fact.EndDate = updatedFact.EndDate;
            fact.IsGoodAction = updatedFact.IsGoodAction;
            fact.Tags = updatedFact.Tags;
            fact.Context = updatedFact.Context;
            fact.Statement = updatedFact.Statement;
            fact.Content = updatedFact.Content;
            fact.Summary = updatedFact.Summary;
            fact.Verdict = updatedFact.Verdict;
            fact.EntityId = updatedFact.EntityId;
            fact.LastUpdatedAt = DateTime.UtcNow;
            fact.LastUpdatedBy = currentUser;

            _context.Facts.Update(fact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFactAsync(Guid id)
        {
            var fact = await _context.Facts.FindAsync(id);
            if (fact == null) return false;

            _context.Facts.Remove(fact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
