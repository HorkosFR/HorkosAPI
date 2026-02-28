namespace HorkosAPI.Search.Models
{
    public class SearchItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }

        public SearchItem() { }

        public SearchItem(Database.Models.Group group)
        {
            Id = group.Id;
            Name = group.Name;
            Description = group.Description;
            ImageUrl = group.ImageUrl;
            Type = "group";
        }

        public SearchItem(Database.Models.Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.ShortBio;
            ImageUrl = entity.ImageUrl;
            Type = "entity";
        }
    }
}
