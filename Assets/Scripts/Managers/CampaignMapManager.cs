using System.Collections.Generic;
using Gangs.Campaign;
using Gangs.Campaign.GameObjects;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignMapManager : MonoBehaviour {
        public static CampaignMapManager Instance { get; private set; }
        
        public CampaignMapGameObject MapGameObject { get; set; }
        public List<CampaignEntityGameObject> Entities { get; set; } = new();
        public List<CampaignTerritoryGameObject> Territories { get; set; } = new();
        
        private void Awake() {
            if (Instance == null) { Instance = this; }
            else { Destroy(gameObject); }
            
            SetMapParent();
        }
        
        public void MoveEntity(CampaignSquad activeSquad, CampaignTerritory hoverTerritory) {
            var entity = Entities.Find(e => e.Entity == activeSquad);
            entity.Move(hoverTerritory);
            ResetTerritoryHighlights();
        }
        
        public CampaignTerritory GetTerritory(CampaignSquad squad) => 
            Territories.Find(t => t.Territory.Entities.Contains(squad));
        
        public void ResetTerritoryHighlights() => Territories.ForEach(t => t.ResetColour());

        public void SetMapParent() => 
            MapGameObject = GameObject.FindGameObjectWithTag("MapParent").GetComponent<CampaignMapGameObject>();
    }
}