using HorkosAPI.Database.Models;

namespace HorkosAPI.Entity.Models
{
    public class EntityGroupDTO
    {
        public Guid GroupId { get; set; }
        public Database.Models.Group Group { get; set; } = null!;

        public Guid EntityId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Database.Models.Entity Entity { get; set; } = null!;

        public EntityGroupDTO(GroupEntity ge)
        {
            GroupId = ge.GroupId;
            Group = ge.Group;
            EntityId = ge.EntityId;
        }

        public EntityGroupDTO() { }
    }
}
