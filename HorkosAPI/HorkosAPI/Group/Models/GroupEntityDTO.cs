using HorkosAPI.Database.Models;

namespace HorkosAPI.Group.Models
{
    public class GroupEntityDTO
    {
        public Guid GroupId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Database.Models.Group Group { get; set; } = null!;

        public Guid EntityId { get; set; }
        public Database.Models.Entity Entity { get; set; } = null!;

        public GroupEntityDTO(GroupEntity ge)
        {
            GroupId = ge.GroupId;
            EntityId = ge.EntityId;
            Entity = ge.Entity;
        }

        public GroupEntityDTO() { }
    }
}
