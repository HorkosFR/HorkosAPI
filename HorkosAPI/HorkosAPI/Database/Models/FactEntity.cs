namespace HorkosAPI.Database.Models
{
    public class FactEntity
    {
        public Guid FactId { get; set; }
        public Fact Fact { get; set; } = null!;

        public Guid EntityId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Entity Entity { get; set; } = null!;
    }
}
