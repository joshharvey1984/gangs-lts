using Gangs.Data;

namespace Gangs.Campaign {
    public class CampaignUnit {
        public Fighter BaseUnit { get; set; }
        
        public CampaignUnit(Fighter unit) {
            BaseUnit = unit;
        }
    }
}