using System.Collections.Generic;
using Gangs.Campaign;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }
        
        private CampaignMap _campaignMap;
        [SerializeField] private GameObject mapParent;
        
        private List<CampaignGang> _gangs;
        private CampaignGang _currentGang;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance is null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        private void Start() {
            var campaignData = new CampaignData {
                CampaignGangs = new List<CampaignGang> {
                    new() { BaseGang = Gang.All[0], IsPlayerControlled = true },
                    new() { BaseGang = Gang.All[1], IsPlayerControlled = false }
                },
                MapSize = CampaignMapSize.Small
            };
            
            _campaignMap = new CampaignMap(campaignData, mapParent);
            _gangs = campaignData.CampaignGangs;
            
            CampaignUIManager.Instance.SetCampaignInfo(campaignData);
            CampaignUIManager.Instance.SetTurnNumberText(1);
            CampaignUIManager.Instance.SetGangTurn(_gangs[0].BaseGang);
            
            _currentGang = _gangs[0];
            StartTurn();
        }
        
        public void StartTurn() {
            _currentGang.StartTurn();
        }

        public CampaignTerritory GetTerritory(CampaignSquad squad) {
            return Instance._campaignMap.Territories.Find(t => t.Entities.Contains(squad));
        }
        
        public CampaignTerritory GetTerritory(CampaignTerritoryGameObject territoryGameObject) {
            return Instance._campaignMap.GetTerritoryByGameObject(territoryGameObject);
        }

        public void ResetTerritoryHighlights() {
            _campaignMap.Territories.ForEach(t => t.GameObject.ResetColour());
        }

        public void BattleMenu(CampaignTerritory territory) {
            CampaignUIManager.Instance.SetBattleMenu(territory);
        }

        public void Battle() {
            var territory = _campaignMap.Territories.Find(t => t.Entities.Count > 1);
            var battle = new CampaignBattle(territory, CampaignBattleType.Auto);
            Debug.Log("Battle");
        }
    }
}