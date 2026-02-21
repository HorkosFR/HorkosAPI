using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Fact.Models;
using HorkosAPI.Source.Models;
using SmartReader;

namespace HorkosAPI.Source.Services
{
    public class SourceService : ISourceService
    {
        private readonly DatabaseContext _context;

        public SourceService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Database.Models.Source>> GetSourcesAsync() =>
            await _context.Sources.ToListAsync();

        public async Task<Database.Models.Source?> GetSourceByIdAsync(Guid id) =>
            await _context.Sources.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<List<Database.Models.Source>> GetSourcesByItemIdAsync(Guid itemId)
        {

            var factCheckLinks = await _context.FactCheckSources
                .Include(fc => fc.Source)
                .Where(fc => fc.FactCheckId == itemId || fc.SourceId == itemId)
                .Select(fc => fc.Source)
                .Where(s => s != null)
                .ToListAsync();
            if (factCheckLinks.Any())
                return factCheckLinks;

            var factLinks = await _context.FactSources
                .Include(fc => fc.Source)
                .Where(fc => fc.FactId == itemId || fc.SourceId == itemId)
                .Select(fc => fc.Source)
                .Where(s => s != null)
                .ToListAsync();
            return factCheckLinks;
        }

        public async Task<Database.Models.Source> CreateSourceAsync(SourceDTO dto, Guid currentUser)
        {
            var source = new Database.Models.Source(dto);
            source.CreatedBy = currentUser;
            source.LastUpdatedBy = currentUser;
            _context.Sources.Add(source);

            var factCheckExists = await _context.FactChecks.AnyAsync(c => c.Id == dto.TargetId);
            if (factCheckExists)
            {
                var link = new Database.Models.FactCheckSource
                {
                    FactCheckId = dto.TargetId,
                    SourceId = source.Id
                };
                _context.FactCheckSources.Add(link);
                await _context.SaveChangesAsync();
                return source;
            }

            var factExists = await _context.Facts.AnyAsync(c => c.Id == dto.TargetId);
            if (factExists)
            {
                var link = new Database.Models.FactSource
                {
                    FactId = dto.TargetId,
                    SourceId = source.Id
                };
                _context.FactSources.Add(link);
                await _context.SaveChangesAsync();
                return source;
            }

            return source;
        }

        public async Task<ItemSourceDTO> GetSourceFromUrlAsync(string url)
        {
            ItemSourceDTO newSource = new();
            if (string.IsNullOrEmpty(url)) throw new Exception("Missing URL.");

            var reader = new Reader(url);
            var article = await reader.GetArticleAsync();

            if (!article.IsReadable)
                throw new Exception("Wrong URL.");

            newSource.Author = article.Byline;
            newSource.Url = url;
            newSource.Title = article.Title;
            newSource.PublicationDate = article.PublicationDate ?? DateTime.UtcNow;
            newSource.SourceType = "Article";
            return newSource;
        }

        public async Task<bool> UpdateSourceAsync(Guid id, SourceDTO dto, Guid currentUser)
        {
            var source = await _context.Sources.FindAsync(id);
            if (source == null) return false;

            source.Title = dto.Title;
            source.Url = dto.Url;
            source.Author = dto.Author;
            source.PublicationDate = dto.PublicationDate;
            source.SourceType = dto.SourceType.ToString();
            source.LastUpdatedBy = currentUser;
            source.LastUpdatedAt = DateTime.UtcNow;
            _context.Sources.Update(source);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSourceAsync(Guid id)
        {
            var source = await _context.Sources.FindAsync(id);
            if (source == null) return false;

            _context.Sources.Remove(source);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
