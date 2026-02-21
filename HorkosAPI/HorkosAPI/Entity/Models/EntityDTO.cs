using HorkosAPI.Database.Models;

namespace HorkosAPI.Entity.Models
{
    public class EntityDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string? ShortBio { get; set; }

        public string? Sector { get; set; }

        public string? Country { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? FoundedDate { get; set; }

        public string? OfficialLinks { get; set; }
        public string? ImageUrl { get; set; }
        public byte[]? Image { get; set; }
        public string? ImageMimeType { get; set; }
        public List<Guid> GroupId {  get; set; }
    }

    public class GetEntityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string? ShortBio { get; set; }
        public string? Sector { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
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

        public ICollection<FactEntity> FactEntities { get; set; } = new List<FactEntity>();
        public ICollection<EntityGroupDTO> EntityGroups { get; set; } = new List<EntityGroupDTO>();

        public GetEntityDTO(Database.Models.Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Type = entity.Type;
            ShortBio = entity.ShortBio;
            Sector = entity.Sector;
            Country = entity.Country;
            CreatedAt = entity.CreatedAt;
            BirthDate = entity.BirthDate;
            FoundedDate = entity.FoundedDate;
            OfficialLinks = entity.OfficialLinks;
            ReliabilityScore = entity.ReliabilityScore;
            ReliabilityVoteAmount = entity.ReliabilityVoteAmount;
            TotalWeightedSum = entity.TotalWeightedSum;
            TotalWeight = entity.TotalWeight;
            ImageUrl = entity.ImageUrl;
            Image = entity.Image;
            ImageMimeType = entity.ImageMimeType;
            IsVisible = entity.IsVisible;
            FactEntities = entity.FactEntities;
            foreach (GroupEntity ge in entity.GroupEntities)
            {
                EntityGroups.Add(new(ge));
            }
        }
    }
}
