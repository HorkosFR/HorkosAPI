namespace HorkosAPI.UserContribution.Models
{
    public class UserContributionDTO
    {
        public Guid UserId { get; set; }

        public string ContributionType { get; set; } = "";

        public Guid ContributionId { get; set; }

        public string Action { get; set; } = "";
        public string IpAddress { get; set; } = "";

        public DateTime Timestamp { get; set; }
    }
}
