using HorkosAPI.Source.Models;

namespace HorkosAPI.Fact.Models
{
    public class FactDTO
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsGoodAction { get; set; }
        public string Tags { get; set; }
        public string Context { get; set; }
        public string Statement { get; set; }
        public string Content { get; set; } = "";
        public string? Summary { get; set; }
        public string? Verdict { get; set; } = "";
        public Guid EntityId { get; set; }
        public List<ItemSourceDTO> Source { get; set; } 
    }
}
