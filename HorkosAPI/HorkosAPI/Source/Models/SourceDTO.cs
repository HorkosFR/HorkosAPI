using HorkosAPI.Database.Models;

namespace HorkosAPI.Source.Models
{
    public class SourceDTO
    {
        public string Title { get; set; } = "";
        public string SourceType { get; set; }
        public string? Author { get; set; }
        public string Url { get; set; } = "";
        public DateTime PublicationDate { get; set; }
        public Guid TargetId { get; set; }
    }

    public class ItemSourceDTO
    {
        public string Title { get; set; } = "";
        public string SourceType { get; set; }
        public string? Author { get; set; }
        public string Url { get; set; } = "";
        public DateTime PublicationDate { get; set; }
    }
}
