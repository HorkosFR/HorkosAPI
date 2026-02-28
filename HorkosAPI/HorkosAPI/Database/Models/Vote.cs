namespace HorkosAPI.Database.Models;

using HorkosAPI.Vote.Models;
using System;

public class Vote
{
    public Guid Id { get; set; }

    public Guid TargetId { get; set; }

    public Guid UserId { get; set; }

    public double Value { get; set; }

    public string Type { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Vote() { }

    public Vote(VoteDTO newVote)
    {
        Id = Guid.NewGuid();
        TargetId = newVote.TargetId;
        UserId = newVote.UserId;
        Value = newVote.Value;
        Type = newVote.Type;
        UpdatedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}

