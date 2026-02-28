using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using Microsoft.EntityFrameworkCore;

namespace HorkosAPI.Gamification.Services
{
    public class GamificationService
    {
        private readonly DatabaseContext _context;

        public GamificationService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AwardPointsAsync(Guid userId, string actionCode, Guid? contributionId = null)
        {
            var action = await _context.GamificationActions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ActionCode == actionCode && a.IsActive);

            if (action == null)
                return;

            var history = new UserActionHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActionId = action.Id,
                ContributionId = contributionId,
                PointsAwarded = action.Points,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserActionHistories.Add(history);

            var userPoints = await _context.UserPoints.FirstOrDefaultAsync(up => up.UserId == userId);
            if (userPoints == null)
            {
                userPoints = new UserPoints
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TotalPoints = 0,
                    CurrentLevel = 1,
                };
                _context.UserPoints.Add(userPoints);
            }

            userPoints.TotalPoints += action.Points;

            var newLevel = await _context.Levels
                .Where(l => l.PointsRequired <= userPoints.TotalPoints)
                .OrderByDescending(l => l.LevelNumber)
                .FirstOrDefaultAsync();

            if (newLevel != null && newLevel.LevelNumber != userPoints.CurrentLevel)
                userPoints.CurrentLevel = newLevel.LevelNumber;

            await _context.SaveChangesAsync();
        }
    }
}
