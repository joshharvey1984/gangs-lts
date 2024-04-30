using Gangs.Data;
using UnityEngine;

namespace Gangs.Campaign.CampaignGenerators {
    public static class CampaignGangGenerator {
        public static CampaignGang GenerateGang(Faction faction) {
            int[] levelOfUnits = {5, 2, 1, 1};
            var gang = new CampaignGang {
                Name = faction.Name,
                Faction = faction
            };
            
            foreach (var t in levelOfUnits) {
                var unitType = faction.Units[Random.Range(0, faction.Units.Count)];
                var unit = CampaignUnitGenerator.GenerateUnit(faction, unitType, t);
                gang.Units.Add(unit);
            }
            
            return gang;
        }
    }
}