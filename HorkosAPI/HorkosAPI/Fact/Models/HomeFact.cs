using HorkosAPI.Source.Models;

namespace HorkosAPI.Fact.Models
{
    public class HomeFact
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string Title { get; set; }
        public bool IsGoodAction { get; set; }
        public string Tags { get; set; }
        public string? Summary { get; set; }
        public string EntityName { get; set; }
        public string? EntityImage { get; set; }

        public HomeFact (Database.Models.Fact fact)
        {
            Id = fact.Id;
            EntityId = fact.Entity.Id;
            Title = fact.Title;
            IsGoodAction = fact.IsGoodAction;
            Tags = fact.Tags;
            Summary = fact.Summary;
            EntityName = fact.Entity.Name;
            EntityImage = fact.Entity.ImageUrl;
        }

        public HomeFact() { }
    }
}
