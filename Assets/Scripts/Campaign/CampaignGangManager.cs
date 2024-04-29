using System.Collections.Generic;

namespace Gangs.Campaign {
    public class CampaignGangManager {
        public CampaignGang Gang { get; set; }
        public bool IsPlayerControlled { get; set; }
        public List<CampaignSquad> Squads { get; set; } = new();
        public CampaignSquad ActiveSquad { get; set; }

        public void StartTurn() {
            ActiveSquad = Squads[0];
            ActiveSquad.Select();
            
            if (IsPlayerControlled) {
                ActiveSquad.SubscribeToTerritoryClicks();
            }
        }
        
        public void AddSquad(CampaignSquad squad) {
            Squads.Add(squad);
        }
    }
}