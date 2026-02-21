using HorkosAPI.Comment.Models;

namespace HorkosAPI.Comment.Services
{
    public interface ICommentService
    {
        Task<List<Database.Models.Comment>> GetCommentsAsync();
        Task<Database.Models.Comment?> GetCommentByIdAsync(Guid id);
        Task<Database.Models.Comment> CreateCommentAsync(CommentDTO comment, Database.Models.User currentUser);
        Task<bool> UpdateCommentAsync(Guid id, Database.Models.Comment updatedComment);
        Task<bool> DeleteCommentAsync(Guid id);
        Task<List<Database.Models.Comment>> GetCommentsByTargetIdAsync(Guid targetId);

    }
}
