using HorkosAPI.Vote.Models;

namespace HorkosAPI.Vote.Services
{
    public interface IVoteService
    {
        public Task<List<Database.Models.Vote>> GetVotesAsync();
        public Task<Database.Models.Vote?> GetVoteByIdAsync(Guid id);
        public Task<Database.Models.Vote> CreateVoteAsync(VoteDTO newVote);
        public Task<bool> UpdateVoteAsync(Guid id, VoteDTO updatedVote);
        public Task<List<Database.Models.Vote>?> GetVoteByTargetIdCurrentUserAsync(Guid id, Guid currentUserId);
        public Task<List<Database.Models.Vote>?> GetVoteByCommentCurrentUserAsync(Guid id, Guid currentUserId);
        public Task<bool> DeleteVoteAsync(Guid id);
    }
}
