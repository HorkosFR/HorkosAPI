using System;
using System.Collections.Generic;
using System.Linq;

namespace FiabilitePolitique
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

    public class Indicator
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public double Weight { get; set; }

        public List<EvidenceItem> RelatedItems { get; set; } = new();

        public double AdjustedScore
        {
            get
            {
                double adjusted = Score;
                foreach (var item in RelatedItems)
                {
                    adjusted -= Math.Abs(item.WeightedImpact);
                }
                return Math.Clamp(adjusted, 0, 5);
            }
        }
    }

    public class PoliticianProfile
    {
        public string Name { get; set; }
        public List<Indicator> Indicators { get; set; } = new();


        public double GlobalScore
        {
            get
            {
                if (!Indicators.Any()) return 0;
                double weightedSum = Indicators.Sum(i => i.AdjustedScore * i.Weight);
                double totalWeight = Indicators.Sum(i => i.Weight);
                return (weightedSum / totalWeight) * 20;
            }
        }
    }

    public static class Example
    {
        public static void RunDemo()
        {
            var corruptionItem = new EvidenceItem
            {
                Id = "E1",
                Title = "Condamnation pour corruption",
                BaseImpact = -1,
                GravityVotes = new List<double> { 5, 4.5, 5 },
                ReliabilityVotes = new List<double> { 4, 4, 5 }
            };

            var transparencyIndicator = new Indicator
            {
                Id = "I1",
                Name = "Transparence et intégrité",
                Score = 5,
                Weight = 0.2,
                RelatedItems = new List<EvidenceItem> { corruptionItem }
            };

            var profile = new PoliticianProfile
            {
                Name = "Nicolas Sarkozy",
                Indicators = new List<Indicator>
                {
                    transparencyIndicator,
                    new Indicator { Id = "I2", Name = "Identité et parcours", Score = 4.5, Weight = 0.1 },
                    new Indicator { Id = "I3", Name = "Cohérence actions / discours", Score = 2.5, Weight = 0.2 },
                    new Indicator { Id = "I4", Name = "Réputation publique et médiatique", Score = 2.0, Weight = 0.2 },
                    new Indicator { Id = "I5", Name = "Compétence et expérience", Score = 4.0, Weight = 0.1 },
                    new Indicator { Id = "I6", Name = "Relations et alliances", Score = 2.0, Weight = 0.1 },
                    new Indicator { Id = "I7", Name = "Comportement personnel et leadership", Score = 2.0, Weight = 0.1 }
                }
            };

            Console.WriteLine($"Score global pour {profile.Name} : {profile.GlobalScore:F2}/100");
        }
    }
}
