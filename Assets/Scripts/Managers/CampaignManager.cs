using System;
using System.Collections;
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
        
        private List<CampaignEntityGameObject> _entities;
        
        private IBattle _currentBattle;
        private CampaignSquad _victor;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance is null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
            
            SetMapParent();
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
            _entities = _campaignMap.Entities;
            _gangs = campaignData.CampaignGangManagers;
            
            CampaignUIManager.Instance.SetCampaignInfo(campaignData);
            CampaignUIManager.Instance.SetTurnNumberText(1);
            CampaignUIManager.Instance.SetGangTurn(_gangs[0].Gang);
            
            _currentGangManager = _gangs[0];
            StartTurn();
        }
        
        private void SetMapParent() {
            mapParent = GameObject.FindGameObjectWithTag("MapParent");
        }
        
        public void LoadedMap() {
            if (_currentBattle is null) return;
            SetMapParent();
            EndBattle(_victor);
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
        
        public void SelectTerritory(CampaignTerritory hoverTerritory) {
            _currentGangManager.SelectTerritory(hoverTerritory);
        }

        public void MoveEntity(CampaignSquad activeSquad, CampaignTerritory hoverTerritory) {
            var entity = _entities.Find(e => e.Entity == activeSquad);
            entity.Move(hoverTerritory);
            ResetTerritoryHighlights();
            
            if (hoverTerritory.Squads.Count > 1) {
                CampaignInputManager.Instance.InputEnabled = false;
                BattleMenu(hoverTerritory);
            } else {
                StartTurn();
            }
        }

        public void AutoBattle() {
            var territory = GetBattleTerritory();
            _currentBattle = new CampaignBattle(territory);
            _currentBattle.OnEndBattle += AutoEndBattle;
            gameObject.AddComponent<AutoBattleManager>().StartBattle(_currentBattle);
        }

        public void ManualBattle() {
            var territory = GetBattleTerritory();
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
            StartCoroutine(ReLoadCampaign());
            Destroy(GetComponent<BattleStartManager>());
            _victor = victor;
        }
        
        private IEnumerator ReLoadCampaign() {
            var asyncLoad = SceneManager.LoadSceneAsync("Campaign");

            // Wait until the scene has finished loading
            while (!asyncLoad!.isDone) {
                yield return null;
            }
            
            yield return null;
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Campaign"));
        }
        
        private void EndBattle(CampaignSquad victor) {
            _victor = null;
            _currentBattle = null;
            Debug.Log("Battle ended");
            var territory = _campaignMap.Territories.Find(t => t.Squads.Contains(victor));
            var defeated = territory.Squads.Find(s => s != victor);
            var defeatedEntity = _entities.FirstOrDefault(e => e.Entity == defeated);
            if (defeatedEntity) Destroy(defeatedEntity!.gameObject);
            _entities.Remove(defeatedEntity);
            territory.Squads.Remove(defeated);
            CampaignUIManager.Instance.SetBattleMenuVictor(victor);
        }
        
        // TODO: Implement actual territory finding
        private CampaignTerritory GetBattleTerritory() => _campaignMap.Territories.Find(t => t.Squads.Count > 1);
    }
}