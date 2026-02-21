using HorkosAPI.Reliability.Models;

namespace HorkosAPI.Reliability.Services
{
    public class ReliabilityCalculatorService
    {
        private static readonly Dictionary<string, double> TypeWeights = new()
        {
            { "PublicIdentity", 0.1 },
            { "Career", 0.1 },
            { "PastCase", 0.1 },
            { "BelongingsDeclaration", 0.2 },
            { "Scandale", 0.2 },
            { "RulesRespect", 0.2 },
            { "InterestConflict", 0.2 },
            { "Promises", 0.2 },
            { "Stability", 0.2 },
            { "Consistency", 0.2 },
            { "TangibleResult", 0.1 },
            { "Communication", 0.1 },
            { "RelationShip", 0.1 },
            { "Independancy", 0.0 },
            { "PublicAttitude", 0.1 },
            { "EthicalBehavior", 0.1 },
            { "Management", 0.1 }
        };

        private const double TimeDecayRate = 0.1;

        public static ReliabilityResult? ComputeEntityReliability(List<Models.Evidence> evidences)
        {
            if (evidences == null || evidences.Count == 0)
                return null;

            AdjustReliabilitiesWithSources(evidences);

            var relevantEvidences = evidences
                .Where(e => e.Type != "source")
                .ToList();

            double weightedSum = 0.0;
            double totalWeight = 0.0;

            foreach (var e in relevantEvidences)
            {
                if (!TypeWeights.ContainsKey(e.Type.ToLower()))
                    continue;

                double typeWeight = TypeWeights[e.Type.ToLower()];
                double timeWeight = ComputeTimeWeight(e.Date);

                double impactValue;

                if (e.Gravity > 0 && e.Reliability > 0)
                {
                    impactValue = (e.Gravity / 100.0) * (e.Reliability / 100.0);
                }
                else if (e.Gravity > 0)
                {
                    impactValue = (e.Gravity / 100.0);
                }
                else if (e.Reliability > 0)
                {
                    impactValue = 1.0 - (e.Reliability / 100.0);
                }
                else
                {
                    continue;
                }

                double weightedImpact = impactValue * typeWeight * timeWeight;

                weightedSum += weightedImpact;
                totalWeight += typeWeight * timeWeight;
            }

            if (totalWeight == 0)
                return null;

            double normalizedImpact = weightedSum / totalWeight;
            double entityReliability = 100.0 * (1.0 - normalizedImpact);
            ReliabilityResult result = new();
            result.Reliability = Math.Round(Math.Max(0.0, Math.Min(100.0, entityReliability)), 2);
            result.TotalWeight = totalWeight;
            result.TotalWeightedSum = weightedSum;

            return result;
        }

        private static void AdjustReliabilitiesWithSources(List<Models.Evidence> evidences)
        {
            var sources = evidences.Where(e => e.Type == "source").ToList();
            var targets = evidences.Where(e => e.Type == "factcheck" || e.Type == "lawsuit").ToList();

            foreach (var target in targets)
            {
                var linkedSources = sources
                    .Where(s => s.LinkedTo == target.Id)
                    .ToList();

                if (!linkedSources.Any())
                    continue;

                double avgSourceReliability = linkedSources.Average(s => s.Reliability);
                target.Reliability *= (0.5 + 0.5 * (avgSourceReliability / 100.0));

                target.Reliability = Math.Max(0.0, Math.Min(100.0, target.Reliability));
            }
        }

        public static ReliabilityResult? UpdateEntityReliabilityIncremental(
            double currentReliability,
            double totalWeightedSum,
            double totalWeight,
            Models.Evidence newEvidence)
        {
            ReliabilityResult result = new()
            {
                Reliability = currentReliability,
                TotalWeight = totalWeight,
                TotalWeightedSum = totalWeightedSum
            };

            if (!TypeWeights.TryGetValue(newEvidence.Type.ToLower(), out double typeWeight))
                return result;

            double timeWeight = ComputeTimeWeight(newEvidence.Date);
            double wNew = typeWeight * timeWeight;

            double impactValue;

            if (newEvidence.Gravity > 0 && newEvidence.Reliability > 0)
            {
                impactValue = (newEvidence.Gravity / 100.0) * (newEvidence.Reliability / 100.0);
            }
            else if (newEvidence.Gravity > 0)
            {
                impactValue = (newEvidence.Gravity / 100.0);
            }
            else if (newEvidence.Reliability > 0)
            {
                impactValue = 1.0 - (newEvidence.Reliability / 100.0);
            }
            else
            {
                return result;
            }

            double weightedImpactNew = impactValue * wNew;

            double newWeightedSum = totalWeightedSum + weightedImpactNew;
            double newTotalWeight = totalWeight + wNew;

            if (newTotalWeight == 0)
                return null;

            double normalizedImpact = newWeightedSum / newTotalWeight;
            double updatedReliability = 100.0 * (1.0 - normalizedImpact);

            result.Reliability = Math.Max(0.0, Math.Min(100.0, updatedReliability));
            result.TotalWeight = newTotalWeight;
            result.TotalWeightedSum = newWeightedSum;

            return result;
        }


        private static double ComputeTimeWeight(DateTime date)
        {
            double yearsOld = (DateTime.UtcNow - date).TotalDays / 365.0;
            return Math.Exp(-TimeDecayRate * yearsOld);
        }
    }
}
