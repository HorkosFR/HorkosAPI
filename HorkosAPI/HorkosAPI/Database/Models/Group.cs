using Microsoft.AspNetCore.Http.HttpResults;
using HorkosAPI.Entity.Models;
using HorkosAPI.Group.Models;
using System.Diagnostics.Metrics;

namespace HorkosAPI.Database.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public double? ReliabilityScore { get; set; }
        public ICollection<GroupEntity> GroupEntities { get; set; } = new List<GroupEntity>();
        public bool IsVisible { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Group() { }
        public Group(GroupDTO group)
        {
            Id = Guid.NewGuid();
            Name = group.Name;
            Description = group.Description;
            ImageUrl = group.ImageUrl;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
            ReliabilityScore = 0;
            IsVisible = true;
        }
    }
}
