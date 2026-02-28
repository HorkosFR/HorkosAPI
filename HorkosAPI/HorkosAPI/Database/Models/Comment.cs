using HorkosAPI.Comment.Models;

namespace HorkosAPI.Database.Models;

public class Comment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = "";

    public string TargetType { get; set; } = "";
    public Guid TargetId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Guid? ParentCommentId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Comment? ParentComment { get; set; }

    public virtual User? User { get; set; }
    public int? UpVoteAmount { get; set; } = 0;
    public int? DownVoteAmount { get; set; } = 0;
    public int? Score { get; set; } = 0;
    public bool IsVisible { get; set; }

    public Comment() { }

    public Comment(CommentDTO commentDto)
    {
        Id = Guid.NewGuid();
        UserId = commentDto.UserId;
        Content = commentDto.Content;
        TargetType = commentDto.TargetType;
        TargetId = commentDto.TargetId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        ParentCommentId = commentDto.ParentCommentId;
        IsVisible = true;
    }

}

public enum TargetType
{
    Entity,
    Fact
}
