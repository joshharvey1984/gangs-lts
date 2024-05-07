using System.Collections.Generic;
using Gangs.Managers;

namespace Gangs.Campaign {
    public class CampaignSquad  {
        public CampaignEntityGameObject GameObject { get; set; }
        public string Name { get; set; }
        public List<CampaignUnit> Units { get; set; } = new();
        public bool IsPlayerControlled { get; set; }
        
        public void AddUnit(CampaignUnit campaignUnit) {
            Units.Add(campaignUnit);
            Units.Sort((a, b) => b.Level.CompareTo(a.Level));
        }
        
        public void Select() {
            GameObject.Select();
            CampaignInputManager.Instance.SelectSquad(this);
        }
        
        private void HandleLeftClickTerritory(CampaignTerritory territory) {
            var currentTerritory = CampaignManager.Instance.GetTerritory(this);
            if (!currentTerritory.Neighbours.Contains(territory)) return;
            
            currentTerritory.Squads.Remove(this);
            territory.Squads.Add(this);
            Move(territory);
        }

        private void Move(CampaignTerritory territory) {
            GameObject.Move(territory);
            if (territory.Squads.Count > 1) {
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