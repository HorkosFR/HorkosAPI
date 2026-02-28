using HorkosAPI.Fact.Models;

namespace HorkosAPI.Database.Models;

public class Fact
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsGoodAction { get; set; }
    public string Tags { get; set; }
    public string Context { get; set; }
    public string? Statement { get; set; }
    public string Content { get; set; } = "";
    public string? Summary { get; set; }
    public string? Verdict { get; set; } = "";
    public double? ReliabilityScore { get; set; }
    public double? GravityScore { get; set; }
    public int? GravityVoteAmount { get; set; }
    public int? ReliabilityVoteAmount { get; set; }
    public bool IsVisible { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid LastUpdatedBy { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid EntityId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Entity Entity { get; set; } = null!;

    public ICollection<FactCheck> FactChecks { get; set; } = new List<FactCheck>();
    public ICollection<FactSource> FactSources { get; set; } = new List<FactSource>();
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<FactEntity> FactEntities { get; set; } = new List<FactEntity>();


    public Fact() { }

    public Fact(FactDTO factDTO)
    {
        Id = Guid.NewGuid();
        Title = factDTO.Title;
        Type = factDTO.Type;
        StartDate = factDTO.StartDate;
        EndDate = factDTO.EndDate;
        IsGoodAction = factDTO.IsGoodAction;
        Tags = factDTO.Tags;
        Context = factDTO.Context;
        Statement = factDTO.Statement;
        Content = factDTO.Content ?? "";
        Summary = factDTO.Summary;
        Verdict = factDTO.Verdict ?? "";
        EntityId = factDTO.EntityId;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        ReliabilityScore = 0;
        ReliabilityVoteAmount = 0;
        GravityScore = 0;
        GravityVoteAmount = 0;
        IsVisible = true;
    }

    public enum FactType
    {
        Behavior,
        PublicStatement,
        Case,
        Relationship,
        FlipFlop,
        Success,
        Failure,
        Decision,
        FactMisc
    }

    public enum FactTag
    {
        PublicIdentity,
        Career,
        PastCase,
        BelongingsDeclaration,
        Scandale,
        RulesRespect,
        InterestConflict,
        Promises,
        Stability,
        Consistency,
        TangibleResult,
        Communication,
        RelationShip,
        Independancy,
        PublicAttitude,
        EthicalBehavior,
        Management,
    }
}
