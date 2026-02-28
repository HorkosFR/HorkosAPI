namespace HorkosAPI.Database.Models
{
    public class GroupEntity
    {
        public Guid GroupId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Group Group { get; set; } = null!;
        public Guid EntityId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Entity Entity { get; set; } = null!;
    }
}
