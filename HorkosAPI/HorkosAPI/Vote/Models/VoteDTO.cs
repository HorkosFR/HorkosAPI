namespace HorkosAPI.Vote.Models;

public class VoteDTO
{
    public Guid TargetId { get; set; }
    public string TargetType { get; set; }

    public Guid UserId { get; set; }

    public int Value { get; set; }

    public string Type { get; set; }
}

public enum TargetType
{
    Fact,
    Factcheck,
    Comment
}