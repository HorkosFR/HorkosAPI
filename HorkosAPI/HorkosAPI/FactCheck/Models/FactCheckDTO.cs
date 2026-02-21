using HorkosAPI.Source.Models;

namespace HorkosAPI.FactCheck.Models
{
    public class FactCheckDTO
    {
        public Guid FactId { get; set; }
        public string Title { get; set; } = "";

        public string Result { get; set; } = "";

        public string Justification { get; set; } = "";
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }

        public ItemSourceDTO Source { get; set; }
    }
}
