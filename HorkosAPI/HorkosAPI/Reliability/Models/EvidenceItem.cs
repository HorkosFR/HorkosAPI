namespace HorkosAPI.Reliability.Models
{
    public class EvidenceItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> LinkedIndicatorIds { get; set; } = new();
        public double BaseImpact { get; set; }

        public List<double> GravityVotes { get; set; } = new();
        public List<double> ReliabilityVotes { get; set; } = new();

        public double AverageGravity => GravityVotes.Any() ? GravityVotes.Average() : 0;
        public double AverageReliability => ReliabilityVotes.Any() ? ReliabilityVotes.Average() : 0;

        public double WeightedImpact
        {
            get
            {
                double fGravity = AverageGravity / 5.0;
                double fReliability = AverageReliability / 5.0;
                return BaseImpact * fGravity * fReliability;
            }
        }
    }

}
