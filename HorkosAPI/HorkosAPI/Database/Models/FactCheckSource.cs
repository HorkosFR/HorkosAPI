namespace HorkosAPI.Database.Models
{
    public class FactCheckSource
    {
        public Guid FactCheckId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public FactCheck FactCheck { get; set; } = null!;

        public Guid SourceId { get; set; }
        public Source Source { get; set; } = null!;
    }
}
