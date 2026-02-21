using HorkosAPI.UserContribution.Models;

namespace HorkosAPI.Database.Models;

public class UserContribution
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User User { get; set; } = null!;

    // Type de contribution (statement / factcheck / case / source / edit)
    public string ContributionType { get; set; } = "";

    public Guid ContributionId { get; set; }

    // Action effectuée (created / updated / deleted / commented)
    public string Action { get; set; } = "";
    public DateTime Timestamp { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string IpAddress { get; set; }

    public UserContribution() { }

    public UserContribution(UserContributionDTO userContributionDto)
    {
        Id = Guid.NewGuid();
        UserId = userContributionDto.UserId;
        ContributionType = userContributionDto.ContributionType;
        ContributionId = userContributionDto.ContributionId;
        Action = userContributionDto.Action;
        IpAddress = userContributionDto.IpAddress;
        Timestamp = DateTime.UtcNow;
    }

}

public enum ContributionType
{
    Fact,
    Factcheck,
    Source,
    Comment,
    Link,
    Entity
}

public enum ContributionAction
{
    Created,
    Updated,
    Deleted
}
