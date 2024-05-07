using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Campaign;
using Gangs.Campaign.CampaignGenerators;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }
        
        private CampaignMap _campaignMap;
        [SerializeField] private GameObject mapParent;
        
        private List<CampaignGangManager> _gangs;
        private CampaignGangManager _currentGangManager;
        
        private IBattle _currentBattle;
        
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
        
        private void SetMapParent() => GameObject.FindWithTag("MapParent");
        
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

        public void AutoBattle() {
            var territory = GetTerritory();
            _currentBattle = new CampaignBattle(territory);
            _currentBattle.OnEndBattle += AutoEndBattle;
            gameObject.AddComponent<AutoBattleManager>().StartBattle(_currentBattle);
        }

        public void ManualBattle() {
            var territory = GetTerritory();
            _currentBattle = new CampaignBattle(territory);
            _currentBattle.OnEndBattle += ManualEndBattle;
            _currentBattle.BattleBase.Squads.ForEach(s => s.OnUnitStartTurn += BattleAI.TakeTurn);
            gameObject.AddComponent<BattleStartManager>().SetBattle(_currentBattle);
        }
        
        private void AutoEndBattle(CampaignSquad victor) {
            Destroy(GetComponent<AutoBattleManager>());
            EndBattle(victor);
        }
        
        private void ManualEndBattle(CampaignSquad victor) {
            SceneManager.LoadScene("Campaign");
            SetMapParent();
            Destroy(GetComponent<BattleStartManager>());
            EndBattle(victor);
        }
        
        private void EndBattle(CampaignSquad victor) {
            var defeated = _campaignMap.Territories.Find(t => !t.Squads.Contains(victor)).Squads.Find(s => !s.Equals(victor));
            _campaignMap.Territories.Find(t => t.Squads.Contains(defeated)).Squads.Remove(defeated);
            ResetTerritoryHighlights();
            CampaignUIManager.Instance.SetBattleMenuVictor(victor);
        }
        
        // TODO: Implement actual territory finding

        private CampaignTerritory GetTerritory() => _campaignMap.Territories.Find(t => t.Squads.Count > 1);
    }
}