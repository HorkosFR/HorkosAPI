namespace HorkosAPI.Comment.Services
{
    using Microsoft.EntityFrameworkCore;
    using HorkosAPI.Comment.Models;
    using HorkosAPI.Database.Models;
    using HorkosAPI.Database.Services;
    using HorkosAPI.Gamification.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly DatabaseContext _context;
        private readonly GamificationService _gamificationService;

        public CommentService(DatabaseContext context)
        {
            _context = context;
            _gamificationService = new GamificationService(context);
        }

        /*
        public async Task<List<Database.Models.Comment>> GetCommentsAsync()
        {
            return _json.Comments.ToList();

        }

        public Task<Database.Models.Comment?> GetCommentByIdAsync(Guid id)
        {
            var comment = _json.Comments.FirstOrDefault(c => c.Id == id);

            if (comment != null)
            {
                comment.User = _json.Users
                    .First(u => u.Id == id);
            }
            return Task.FromResult(comment);
        }

        public Task<List<Database.Models.Comment>> GetCommentsByTargetIdAsync(Guid targetId)
        {
            var result = _json.Comments.Where(c => c.TargetId == targetId).ToList();

            foreach (var comment in result)
            {
                comment.User = _json.Users
                    .First(u => comment.UserId == u.Id);
            }

            return Task.FromResult(result);
        }
        */

        
        public async Task<List<Database.Models.Comment>> GetCommentsAsync() =>
            await _context.Comments
                          .Include(c => c.User)
                          .ToListAsync();

        public async Task<Database.Models.Comment?> GetCommentByIdAsync(Guid id) =>
            await _context.Comments
                          .Include(c => c.User)
                          .FirstOrDefaultAsync(c => c.Id == id && c.IsVisible);

        public async Task<List<Database.Models.Comment>> GetCommentsByTargetIdAsync(Guid targetId) =>
            await _context.Comments
                          .Include(c => c.User)
                          .Where(c => c.TargetId == targetId && c.IsVisible)
                          .OrderBy(c => c.CreatedAt)
                          .ToListAsync();

        public async Task<Database.Models.Comment> CreateCommentAsync(CommentDTO comment, Database.Models.User currentUser)
        {
            Database.Models.Comment newComment = new(comment);

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            if (newComment.UserId != Guid.Empty)
                await _gamificationService.AwardPointsAsync(newComment.UserId, "COMMENT_CREATED", newComment.Id);
            newComment.User = currentUser;
            return newComment;
        }

        public async Task<bool> UpdateCommentAsync(Guid id, Database.Models.Comment updatedComment)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            comment.Content = updatedComment.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
