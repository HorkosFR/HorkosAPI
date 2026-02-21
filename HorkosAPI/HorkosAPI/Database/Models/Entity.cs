using HorkosAPI.Entity.Models;
using System.Linq;

namespace HorkosAPI.Database.Models;

public class Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // person / company
    public string? ShortBio { get; set; }
    public string? Sector { get; set; }
    public string? Country { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? FoundedDate { get; set; }
    public string? OfficialLinks { get; set; }
    public double? ReliabilityScore { get; set; }
    public int? ReliabilityVoteAmount { get; set; }
    public double TotalWeightedSum { get; set; }
    public double TotalWeight { get; set; }
    public string? ImageUrl { get; set; }
    public byte[]? Image { get; set; }
    public string? ImageMimeType { get; set; }
    public bool IsVisible { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid LastUpdatedBy { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<FactEntity> FactEntities { get; set; } = new List<FactEntity>();
    public ICollection<GroupEntity> GroupEntities { get; set; } = new List<GroupEntity>();

    public Entity() { }

    public Entity(EntityDTO entity)
    {
        Id = Guid.NewGuid();
        Name = entity.Name;
        Type = entity.Type;
        ShortBio = entity.ShortBio;
        Sector = entity.Sector;
        Country = entity.Country;
        BirthDate = entity.BirthDate;
        FoundedDate = entity.FoundedDate;
        OfficialLinks = entity.OfficialLinks;
        TotalWeight = 0;
        TotalWeightedSum = 0;
        ReliabilityVoteAmount = 0;
        ReliabilityScore = 75;
        ImageUrl = entity.ImageUrl;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        IsVisible = true;
    }
}
