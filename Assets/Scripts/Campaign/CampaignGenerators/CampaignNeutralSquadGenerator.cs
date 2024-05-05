using System.Collections.Generic;
using Gangs.Data;
using Random = UnityEngine.Random;

namespace Gangs.Campaign.CampaignGenerators {
    public static class CampaignNeutralSquadGenerator {
        public static CampaignSquad GenerateNeutralSquad(Faction faction, int squadStrength) {
            if (faction.Playable) {
                throw new System.Exception($"Neutral squad cannot be playable: {faction.Name}");
            }

            var squad = new CampaignSquad();
            var factionUnit = faction.Units[Random.Range(0, faction.Units.Count)];
            
            var unitLevels = GenerateStrengthDistribution(squadStrength, 3, 6);
            foreach (var unitLevel in unitLevels) {
                var unit = CampaignUnitGenerator.GenerateUnit(faction, factionUnit, unitLevel);
                squad.Units.Add(unit);
            }

            return squad;
        }

        private static List<int> GenerateStrengthDistribution(int target, int minElements, int maxElements) {
            var random = new System.Random();
            var numbers = new List<int>();

            while (target > 0 && numbers.Count < maxElements) {
                var nextNumber = random.Next(1, target);
                numbers.Add(nextNumber);
                target -= nextNumber;
            }

            while (numbers.Count < minElements) {
                numbers.Add(1);
                target--;
            }

            if (target <= 0) return numbers;
            for (int i = 0; i < numbers.Count && target > 0; i++) {
                numbers[i]++;
                target--;
            }

            return numbers;
        }
    }
}