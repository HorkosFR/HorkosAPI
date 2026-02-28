using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Services;
using HorkosAPI.UserContribution.Models;
using System;

namespace HorkosAPI.UserContribution.Services;

public class UserContributionService : IUserContributionService
{
    private readonly DatabaseContext _context;

    public UserContributionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Database.Models.UserContribution>> GetContributionsAsync() =>
        await _context.UserContributions
                      .Include(uc => uc.User)
                      .ToListAsync();

    public async Task<Database.Models.UserContribution?> GetContributionByIdAsync(Guid id) =>
        await _context.UserContributions
                      .Include(uc => uc.User)
                      .FirstOrDefaultAsync(uc => uc.Id == id);

    public async Task<Database.Models.UserContribution> CreateContributionAsync(UserContributionDTO contribution)
    {
        Database.Models.UserContribution newContribution = new Database.Models.UserContribution(contribution);
        _context.UserContributions.Add(newContribution);
        await _context.SaveChangesAsync();
        return newContribution;
    }

    public async Task<bool> UpdateContributionAsync(Guid id, Database.Models.UserContribution updatedContribution)
    {
        var contribution = await _context.UserContributions.FindAsync(id);
        if (contribution == null) return false;

        contribution.Action = updatedContribution.Action;
        contribution.ContributionType = updatedContribution.ContributionType;
        contribution.ContributionId = updatedContribution.ContributionId;
        contribution.Timestamp = updatedContribution.Timestamp;
        contribution.UserId = updatedContribution.UserId;

        _context.UserContributions.Update(contribution);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteContributionAsync(Guid id)
    {
        var contribution = await _context.UserContributions.FindAsync(id);
        if (contribution == null) return false;

        _context.UserContributions.Remove(contribution);
        await _context.SaveChangesAsync();
        return true;
    }
}
