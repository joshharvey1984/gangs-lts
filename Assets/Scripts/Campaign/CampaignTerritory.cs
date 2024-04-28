using System.Collections.Generic;
using Gangs.Data;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Campaign {
    public class CampaignTerritory {
        public CampaignTerritoryGameObject GameObject { get; set; }
        public List<CampaignTerritory> Neighbours { get; set; } = new();
        public Territory Territory { get; set; }
        public Map Map { get; set; }
        public CampaignGang ClaimedBy { get; set; }
        public bool Headquarters { get; set; }
        public List<ICampaignEntity> Entities { get; set; } = new();
        
        public CampaignTerritory(GameObject gameObject) {
            GameObject = gameObject.GetComponent<CampaignTerritoryGameObject>();
        }
        
        public void AddEntity(ICampaignEntity entity) {
            Entities.Add(entity);
        }
        
        public void SetTerritory(Territory territory) {
            Territory = territory;
            Map = Map.All[0];
        }
        
        public void SetClaimedBy(CampaignGang gang, bool isHeadquarters = false) {
            ClaimedBy = gang;
            Headquarters = isHeadquarters;
            GameObject.SetColour(gang.BaseGang.Faction.Color);
        }
        
        public void MouseEnter() {
            CampaignUIManager.Instance.SetTerritoryInfo(this);
        }

        public void SpawnEntities() {
            Entities.ForEach(e => GameObject.SpawnEntity(e));
        }
    }
}