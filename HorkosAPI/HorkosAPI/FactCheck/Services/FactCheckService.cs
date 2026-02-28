using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.FactCheck.Models;
using HorkosAPI.Gamification.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorkosAPI.FactCheck.Services
{
    public class FactCheckService : IFactCheckService
    {
        private readonly DatabaseContext _context;
        private readonly GamificationService _gamificationService;

        public FactCheckService(DatabaseContext context)
        {
            _context = context;
            _gamificationService = new GamificationService(context);
        }

        public async Task<List<Database.Models.FactCheck>> GetFactChecksAsync() =>
            await _context.FactChecks
                          .Include(fc => fc.FactCheckSources)
                          .ToListAsync();

        public async Task<Database.Models.FactCheck?> GetFactCheckByIdAsync(Guid id) =>
            await _context.FactChecks
                          .Include(fc => fc.FactCheckSources)
                          .FirstOrDefaultAsync(fc => fc.Id == id);

        public async Task<List<Database.Models.FactCheck>> GetFactChecksByFactIdAsync(Guid factId) =>
            await _context.FactChecks
                          .Include(fc => fc.FactCheckSources)
                          .Where(fc => fc.FactId == factId)
                          .ToListAsync();

        public async Task<Database.Models.FactCheck> CreateFactCheckAsync(FactCheckDTO factCheck, Guid currentUser)
        {
            Database.Models.FactCheck newFactCheck = new(factCheck);
            newFactCheck.LastUpdatedBy = currentUser;
            if (factCheck.Source != null)
            {
                Database.Models.Source source = new(factCheck.Source);
                source.CreatedBy = currentUser;
                source.LastUpdatedBy = currentUser;
                _context.Sources.Add(source);

                var factCheckSource = new Database.Models.FactCheckSource
                {
                    FactCheckId = newFactCheck.Id,
                    SourceId = source.Id
                };

                _context.FactCheckSources.Add(factCheckSource);
            }

            _context.FactChecks.Add(newFactCheck);
            await _context.SaveChangesAsync();

            if (newFactCheck.UserId != Guid.Empty)
                await _gamificationService.AwardPointsAsync(newFactCheck.UserId, "FACTCHECK_CREATED", newFactCheck.Id);

            return newFactCheck;
        }

        public async Task<bool> UpdateFactCheckAsync(Guid id, FactCheckDTO updatedFactCheck, Guid currentUser)
        {
            var factCheck = await _context.FactChecks.FindAsync(id);
            if (factCheck == null) return false;

            factCheck.Title = updatedFactCheck.Title;
            factCheck.Result = updatedFactCheck.Result;
            factCheck.Justification = updatedFactCheck.Justification;
            factCheck.Date = updatedFactCheck.Date;
            factCheck.LastUpdatedBy = currentUser;
            factCheck.LastUpdatedAt = DateTime.UtcNow;
            _context.FactChecks.Update(factCheck);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFactCheckAsync(Guid id)
        {
            var factCheck = await _context.FactChecks.FindAsync(id);
            if (factCheck == null) return false;

            _context.FactChecks.Remove(factCheck);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
