using HorkosAPI.FactCheck.Models;

namespace HorkosAPI.Database.Models;
public class FactCheck
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Result { get; set; } = "";
    public string Justification { get; set; } = "";
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int? UpVoteAmount { get; set; }
    public int? DownVoteAmount { get; set; }
    public int? Score { get; set; }
    public Guid FactId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Fact Fact { get; set; } = null!;
    public bool IsVisible { get; set; }
    public Guid LastUpdatedBy { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<FactCheckSource> FactCheckSources { get; set; } = new List<FactCheckSource>();

    public FactCheck() { }

    public FactCheck(FactCheckDTO factCheckDto)
    {
        Id = Guid.NewGuid();
        Title = factCheckDto.Title;
        FactId = factCheckDto.FactId;
        Result = factCheckDto.Result;
        Justification = factCheckDto.Justification;
        Date = factCheckDto.Date;
        UserId = factCheckDto.UserId;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        DownVoteAmount = 0;
        UpVoteAmount = 0;
        Score = 0;
        IsVisible = true;
    }

}

public enum FactCheckStatus
{
    True,
    False,
    Misleading,
    Unverifiable,
    New
}

public enum FactCheckResult
{
    True,
    False,
    PartiallyTrue,
    Misleading,
    Unproven,
    OutOfContext
}

