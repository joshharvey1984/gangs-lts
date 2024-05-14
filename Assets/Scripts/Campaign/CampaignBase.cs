using Gangs.MainMenu;

namespace Gangs.Campaign {
    public class CampaignBase {
        public CampaignMap CampaignMap { get; set; }
        
        public CampaignBase() {
            
        }
        
        public CampaignTerritory FindTerritoryBySquad(CampaignSquad squad) => 
            CampaignMap.Territories.Find(t => t.Squads.Contains(squad));
        
        
    }
}