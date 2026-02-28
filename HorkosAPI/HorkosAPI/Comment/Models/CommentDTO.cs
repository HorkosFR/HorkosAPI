namespace HorkosAPI.Comment.Models
{
    public class CommentDTO
    {
        public Guid UserId { get; set; }

        public string Content { get; set; } = "";

        public string TargetType { get; set; } = "";
        public Guid TargetId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid? ParentCommentId { get; set; }



    }
}
