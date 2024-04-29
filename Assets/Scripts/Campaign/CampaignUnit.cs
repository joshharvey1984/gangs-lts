using Gangs.Data;

namespace Gangs.Campaign {
    public class CampaignUnit {
        public Unit BaseUnit { get; set; }
        
        public CampaignUnit(Unit unit) {
            BaseUnit = unit;
        }
    }
}