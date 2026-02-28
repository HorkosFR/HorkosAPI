using HorkosAPI.Database.Models;

namespace HorkosAPI.Group.Models
{
    public class GroupDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid EntityId { get; set; }
    }

    public class GetGroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public double? ReliabilityScore { get; set; }
        public ICollection<GroupEntityDTO> GroupEntities { get; set; } = new List<GroupEntityDTO>();
        public bool IsVisible { get; set; }

        public GetGroupDTO() { }
        public GetGroupDTO(Database.Models.Group group)
        {
            Id = group.Id;
            Name = group.Name;
            Description = group.Description;
            ImageUrl = group.ImageUrl;
            ReliabilityScore = group.ReliabilityScore;
            IsVisible = group.IsVisible;
            foreach (var ge in group.GroupEntities)
            {
                GroupEntities.Add(new(ge));
            }
        }
    }
}
