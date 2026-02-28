using Microsoft.AspNetCore.Http.HttpResults;
using HorkosAPI.Source.Models;

namespace HorkosAPI.Database.Models;

public class Source
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string SourceType { get; set; }
    public string? Author { get; set; }
    public string Url { get; set; } = "";
    public DateTime PublicationDate { get; set; }
    public double? ReliabilityScore { get; set; }
    public int ReliabilityVoteAmount { get; set; }
    public bool IsVisible { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid LastUpdatedBy { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<FactSource> FactSources { get; set; } = new List<FactSource>();
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<FactCheckSource> FactCheckSources { get; set; } = new List<FactCheckSource>();

    public Source() { }

    public Source(SourceDTO sourceDto)
    {
        Id = Guid.NewGuid();
        Title = sourceDto.Title;
        SourceType = sourceDto.SourceType.ToString();
        Author = sourceDto.Author;
        Url = sourceDto.Url;
        PublicationDate = sourceDto.PublicationDate;
        IsVisible = true;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        ReliabilityScore = 0;
        ReliabilityVoteAmount = 0;
    }

    public Source(ItemSourceDTO sourceDto)
    {
        Id = Guid.NewGuid();
        Title = sourceDto.Title;
        SourceType = sourceDto.SourceType.ToString();
        Author = sourceDto.Author;
        Url = sourceDto.Url;
        PublicationDate = sourceDto.PublicationDate;
        IsVisible = true;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        ReliabilityScore = 0;
        ReliabilityVoteAmount = 0;
    }

}

public enum SourceType
{
    Article,
    Video,
    CourtRecord
}