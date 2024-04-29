using Gangs.Data;

namespace Gangs.Campaign.GangGenerator {
    public static class CampaignGangGenerator {
        public static CampaignGang GenerateGang(Faction faction) {
            int[] levelOfUnits = {4, 2, 1, 1};
            var gang = new CampaignGang {
                Name = faction.Name
            };
            
            foreach (var t in levelOfUnits) {
                var unit = CampaignUnitGenerator.GenerateUnit(faction, t);
                gang.Units.Add(unit);
            }
            
            return gang;
        }
    }
}