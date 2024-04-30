using System.Collections.Generic;
using System.Linq;
using Gangs.Campaign;
using Gangs.Campaign.CampaignGenerators;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }
        
        private CampaignMap _campaignMap;
        [SerializeField] private GameObject mapParent;
        
        private List<CampaignGangManager> _gangs;
        private CampaignGangManager _currentGangManager;
        
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
                CampaignGangManagers = new List<CampaignGangManager> {
                    new() { Gang = CampaignGangGenerator.GenerateGang(Faction.All.Where(f => f.Playable).ToList()[0]), IsPlayerControlled = true },
                    new() { Gang = CampaignGangGenerator.GenerateGang(Faction.All.Where(f => f.Playable).ToList()[1]), IsPlayerControlled = true }
                },
                MapSize = CampaignMapSize.Small
            };
            
            _campaignMap = new CampaignMap(campaignData, mapParent);
            _gangs = campaignData.CampaignGangManagers;
            
            CampaignUIManager.Instance.SetCampaignInfo(campaignData);
            CampaignUIManager.Instance.SetTurnNumberText(1);
            CampaignUIManager.Instance.SetGangTurn(_gangs[0].Gang);
            
            _currentGangManager = _gangs[0];
            StartTurn();
        }
        
        public void StartTurn() {
            _currentGangManager.StartTurn();
        }

        public CampaignTerritory GetTerritory(CampaignSquad squad) {
            return Instance._campaignMap.Territories.Find(t => t.Squads.Contains(squad));
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
            var territory = _campaignMap.Territories.Find(t => t.Squads.Count > 1);
            var battle = new CampaignBattle(territory, CampaignBattleType.Auto);
            Debug.Log("Battle");
        }
    }
}