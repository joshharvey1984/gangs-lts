using System.Collections.Generic;
using Gangs.Managers;

namespace Gangs.Campaign {
    public class CampaignGangManager {
        public CampaignGang Gang { get; set; }
        public bool IsPlayerControlled { get; set; }
        public List<CampaignSquad> Squads { get; set; } = new();
        public CampaignSquad ActiveSquad { get; set; }

        public void StartTurn() {
            ActiveSquad = Squads[0];
            ActiveSquad.Select(true);
            
            if (IsPlayerControlled) {
                CampaignInputManager.Instance.SelectSquad(ActiveSquad);
            }
        }
        
        public void AddSquad(CampaignSquad squad) {
            Squads.Add(squad);
        }

        public void SelectTerritory(CampaignTerritory hoverTerritory) {
            if (ActiveSquad is null) return;
            var activeSquadTerritory = CampaignManager.Instance.GetTerritory(ActiveSquad);
            if (activeSquadTerritory == hoverTerritory) return;
            if (!hoverTerritory.Neighbours.Contains(activeSquadTerritory)) return;
            
            activeSquadTerritory.Squads.Remove(ActiveSquad);
            hoverTerritory.AddEntity(ActiveSquad);
            
            CampaignManager.Instance.MoveEntity(ActiveSquad, hoverTerritory);
        }
    }
}