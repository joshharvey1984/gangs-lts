using Gangs.Data;

namespace Gangs.Campaign {
    public class CampaignBattle {
        public Grid.Grid Grid { get; private set; }
        
        public CampaignBattle(CampaignTerritory territory) {
            Grid = new Grid.Grid(10, 10, 10);
        }
    }
}