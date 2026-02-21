namespace HorkosAPI.Database.Models
{
    public class FactSource
    {
        public Guid FactId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Fact Fact { get; set; } = null!;

        public Guid SourceId { get; set; }
        public Source Source { get; set; } = null!;
    }
}
