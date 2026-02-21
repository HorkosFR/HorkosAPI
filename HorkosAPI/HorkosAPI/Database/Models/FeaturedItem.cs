using HorkosAPI.Search.Models;

namespace HorkosAPI.Database.Models
{
    public class FeaturedItem
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SearchItem Item { get; set; }
    }
}
