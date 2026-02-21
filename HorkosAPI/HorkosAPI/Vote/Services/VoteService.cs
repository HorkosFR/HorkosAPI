using global::HorkosAPI.Database.Services;
using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Models;
using HorkosAPI.Reliability.Models;
using HorkosAPI.Reliability.Services;
using HorkosAPI.Vote.Helpers;
using HorkosAPI.Vote.Models;
using HorkosAPI.Gamification.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorkosAPI.Vote.Services
{
    public class VoteService : IVoteService
    {
        private readonly DatabaseContext _context;
        private readonly GamificationService _gamificationService;

        public VoteService(DatabaseContext context)
        {
            _context = context;
            _gamificationService = new GamificationService(context);
        }

        public async Task<List<Database.Models.Vote>> GetVotesAsync() =>
            await _context.Votes.AsNoTracking().ToListAsync();

        public async Task<Database.Models.Vote?> GetVoteByIdAsync(Guid id) =>
            await _context.Votes.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        public async Task<List<Database.Models.Vote>?> GetVoteByTargetIdCurrentUserAsync(Guid id, Guid currentUserId)
        {
            List<Database.Models.Vote> votes = new();
            votes = await _context.Votes.AsNoTracking().Where(v => v.TargetId == id && v.UserId == currentUserId).ToListAsync();
            List<Database.Models.FactCheck> factChecks = await _context.FactChecks.AsNoTracking().Where(fc => fc.FactId == id).ToListAsync();
            if (factChecks.Count > 0)
            {
                var factCheckIds = factChecks.Select(fc => fc.Id).ToList();
                var factVotes = await _context.Votes
                    .AsNoTracking()
                    .Where(v => factCheckIds.Contains(v.TargetId) && v.UserId == currentUserId)
                    .ToListAsync();
                votes.AddRange(factVotes);
            }
            return votes;
        }

        public async Task<List<Database.Models.Vote>?> GetVoteByCommentCurrentUserAsync(Guid id, Guid currentUserId)
        {
            List<Database.Models.Vote> votes = new();
            List<Database.Models.Comment> comments = await _context.Comments.AsNoTracking().Where(c => c.TargetId == id).ToListAsync();
            if (comments.Count > 0)
            {
                var commentsIds = comments.Select(c => c.Id).ToList();
                var commentVotes = await _context.Votes
                    .AsNoTracking()
                    .Where(v => commentsIds.Contains(v.TargetId) && v.UserId == currentUserId)
                    .ToListAsync();
                votes.AddRange(commentVotes);
            }
            return votes;
        }

        public async Task<Database.Models.Vote> CreateVoteAsync(VoteDTO newVote)
        {
            Database.Models.Vote vote = new(newVote);
            Reliability.Models.Evidence evidence = new();
            List<Database.Models.Entity> entities = new();
            List<Database.Models.FactEntity> factEntities = new();

            switch (newVote.TargetType)
            {
                case "Fact":
                    Database.Models.Fact fact = await _context.Facts.FindAsync(newVote.TargetId)
                        ?? throw new Exception($"Target {newVote.TargetId} item does not exist.");
                    List<Database.Models.FactEntity> fe1 = await _context.FactEntities.Where(fe => fe.FactId == fact.Id).ToListAsync();
                    factEntities.AddRange(fe1);
                    evidence = new(newVote, fact.StartDate, fact.Tags);

                    if (newVote.Type.ToLower().Equals("gravity"))
                    {
                        fact.GravityScore = VoteHelper.UpdateScore(fact.GravityVoteAmount, fact.GravityScore, newVote.Value);
                        fact.GravityVoteAmount += 1;

                        await _gamificationService.AwardPointsAsync(newVote.UserId, "VOTE_GRAVITY", fact.Id);
                    }

                    if (newVote.Type.ToLower().Equals("reliability"))
                    {
                        fact.ReliabilityScore = VoteHelper.UpdateScore(fact.ReliabilityVoteAmount, fact.ReliabilityScore, newVote.Value);
                        fact.ReliabilityVoteAmount += 1;
                    }

                    _context.Facts.Update(fact);
                    break;

                case "Comment":
                    Database.Models.Comment comment = await _context.Comments.FindAsync(newVote.TargetId)
                        ?? throw new Exception($"Target {newVote.TargetId} item does not exist.");

                    if (newVote.Type.ToLower().Equals("upvote"))
                    {
                        comment.UpVoteAmount += 1;
                        comment.Score += 1;
                        _context.Comments.Update(comment);

                        await _gamificationService.AwardPointsAsync(newVote.UserId, "LIKE_GIVEN", comment.Id);

                        if (comment.UserId != Guid.Empty)
                            await _gamificationService.AwardPointsAsync(comment.UserId, "LIKE_RECEIVED", comment.Id);
                    }

                    if (newVote.Type.ToLower().Equals("downvote"))
                    {
                        comment.DownVoteAmount += 1;
                        comment.Score -= 1;
                        _context.Comments.Update(comment);

                        await _gamificationService.AwardPointsAsync(newVote.UserId, "DISLIKE_GIVEN", comment.Id);

                        if (comment.UserId != Guid.Empty)
                            await _gamificationService.AwardPointsAsync(comment.UserId, "DISLIKE_RECEIVED", comment.Id);
                    }

                    _context.Votes.Add(vote);
                    await _context.SaveChangesAsync();
                    return vote;
                case "FactCheck":
                    Database.Models.FactCheck factcheck = await _context.FactChecks.FindAsync(newVote.TargetId)
                        ?? throw new Exception($"Target {newVote.TargetId} item does not exist.");
                    List<Database.Models.FactEntity> fe2 = await _context.FactEntities.Where(fe => fe.FactId == factcheck.FactId).ToListAsync();
                    factEntities.AddRange(fe2);

                    if (newVote.Type.ToLower().Equals("upvote"))
                    {
                        if (factcheck.UpVoteAmount == null)
                            factcheck.UpVoteAmount = 0;
                        if (factcheck.DownVoteAmount == null)
                            factcheck.DownVoteAmount = 0;
                        factcheck.UpVoteAmount += 1;
                        factcheck.Score = (factcheck.UpVoteAmount + factcheck.DownVoteAmount == 0)
                            ? 50
                            : (int)Math.Round((double)(factcheck.UpVoteAmount /
                                              (factcheck.UpVoteAmount + factcheck.DownVoteAmount) * 100));
                        _context.FactChecks.Update(factcheck);

                        await _gamificationService.AwardPointsAsync(newVote.UserId, "LIKE_GIVEN", factcheck.Id);

                        if (factcheck.UserId != Guid.Empty)
                            await _gamificationService.AwardPointsAsync(factcheck.UserId, "LIKE_RECEIVED", factcheck.Id);
                    }

                    if (newVote.Type.ToLower().Equals("downvote"))
                    {
                        if (factcheck.UpVoteAmount == null)
                            factcheck.UpVoteAmount = 0;
                        if (factcheck.DownVoteAmount == null)
                            factcheck.DownVoteAmount = 0;
                        factcheck.DownVoteAmount += 1;
                        factcheck.Score = (factcheck.UpVoteAmount + factcheck.DownVoteAmount == 0)
                            ? 50
                            : (int)Math.Round((double)(factcheck.UpVoteAmount /
                                              (factcheck.UpVoteAmount + factcheck.DownVoteAmount) * 100));
                        _context.FactChecks.Update(factcheck);

                        await _gamificationService.AwardPointsAsync(newVote.UserId, "DISLIKE_GIVEN", factcheck.Id);

                        if (factcheck.UserId != Guid.Empty)
                            await _gamificationService.AwardPointsAsync(factcheck.UserId, "DISLIKE_RECEIVED", factcheck.Id);
                    }

                    _context.Votes.Add(vote);
                    await _context.SaveChangesAsync();
                    return vote;
                default:
                    break;
            }
            var entityIds = factEntities.Select(fe => fe.EntityId).ToList();
            entities = await _context.Entities
                .Where(e => entityIds.Contains(e.Id))
                .ToListAsync();
            foreach (Database.Models.Entity e in entities)
            {
                ReliabilityResult res;

                if (e.TotalWeight == 0 || e.ReliabilityScore == null)
                    res = ReliabilityCalculatorService.ComputeEntityReliability([evidence]) ?? new ReliabilityResult();
                else
                    res = ReliabilityCalculatorService.UpdateEntityReliabilityIncremental((double)e.ReliabilityScore, e.TotalWeightedSum, e.TotalWeightedSum, evidence) ?? new ReliabilityResult();

                e.TotalWeight = res.TotalWeight;
                e.TotalWeightedSum = res.TotalWeightedSum;
                e.ReliabilityScore = res.Reliability;
                _context.Entities.Update(e);
            }

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return vote;
        }

        public async Task<bool> UpdateVoteAsync(Guid id, VoteDTO updatedVote)
        {
            var existingVote = await _context.Votes.FindAsync(id);
            if (existingVote == null)
                return false;

            existingVote.Value = updatedVote.Value;
            existingVote.Type = updatedVote.Type;
            existingVote.UpdatedAt = DateTime.UtcNow;

            _context.Votes.Update(existingVote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVoteAsync(Guid id)
        {
            var existingVote = await _context.Votes.FindAsync(id);
            if (existingVote == null)
                return false;

            _context.Votes.Remove(existingVote);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
