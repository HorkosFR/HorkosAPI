using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using HorkosAPI.Database.Services;
using HorkosAPI.Search.Models;

namespace HorkosAPI.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly DatabaseContext _context;

        public SearchService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<SearchItem>> GetLatestItemsAsync()
        {
            List<SearchItem> items = new();
            List<Database.Models.Group> groups = await _context.Groups
                                                     .OrderByDescending(g => g.Name)
                                                     .Take(3)
                                                     .ToListAsync();

            List<Database.Models.Entity> entities = await _context.Entities
                                         .OrderByDescending(e => e.Name)
                                         .Take(3)
                                         .ToListAsync();

            foreach (var g in groups)
            {
                SearchItem i = new(g);
                items.Add(i);
            }
            foreach (var e in entities)
            {
                SearchItem i = new(e);
                items.Add(i);
            }

            return items;
        }

        public async Task<List<SearchItem>> SearchItemsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<SearchItem>();

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

            var items = groups.Select(g => new SearchItem(g)).ToList();
            items.AddRange(entities.Select(e => new SearchItem(e)));

            return items;
        }
    }
}
