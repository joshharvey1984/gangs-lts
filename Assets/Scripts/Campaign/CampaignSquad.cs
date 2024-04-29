using System.Collections.Generic;
using Gangs.Managers;

namespace Gangs.Campaign {
    public class CampaignSquad : ICampaignEntity {
        public CampaignEntityGameObject GameObject { get; set; }
        public string Name { get; set; }
        public List<CampaignUnit> Units { get; set; } = new();
        
        public CampaignSquad(CampaignGangManager gangManager) {
            Name = gangManager.BaseGang.Name + " Squad";
            gangManager.BaseGang.Fighters.ForEach(f => Units.Add(new CampaignUnit(f)));
        }
        
        public void Select() {
            GameObject.Select();
            CampaignInputManager.Instance.SelectSquad(this);
        }
        
        private void HandleLeftClickTerritory(CampaignTerritory territory) {
            var currentTerritory = CampaignManager.Instance.GetTerritory(this);
            if (!currentTerritory.Neighbours.Contains(territory)) return;
            
            currentTerritory.Entities.Remove(this);
            territory.Entities.Add(this);
            Move(territory);
        }

        private void Move(CampaignTerritory territory) {
            GameObject.Move(territory);
            if (territory.Entities.Count > 1) {
                CampaignManager.Instance.BattleMenu(territory);
            }
            EndTurn();
        }

        private void EndTurn() {
            CampaignInputManager.Instance.OnLeftClickTerritory -= Move;
            CampaignInputManager.DeselectSquad();
        }

        public void SubscribeToTerritoryClicks() {
            CampaignInputManager.Instance.OnLeftClickTerritory += HandleLeftClickTerritory;
        }
    }
}