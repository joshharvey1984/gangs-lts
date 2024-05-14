using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Campaign;
using Gangs.Campaign.CampaignGenerators;
using Gangs.Campaign.GameObjects;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }
        
        public CampaignBase Campaign { get; private set; }
        
        private List<CampaignGangManager> _gangs;
        private CampaignGangManager _currentGangManager;
        
        private IBattle _currentBattle;
        private CampaignSquad _victor;
        
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
            
            Campaign = new CampaignBase();
            Campaign.CampaignMap = new CampaignMap(campaignData);
            
            _gangs = campaignData.CampaignGangManagers;
            
            CampaignUIManager.Instance.SetCampaignInfo(campaignData);
            CampaignUIManager.Instance.SetTurnNumberText(1);
            CampaignUIManager.Instance.SetGangTurn(_gangs[0].Gang);
            
            _currentGangManager = _gangs[0];
            StartTurn();
        }

        public void LoadedMap() {
            if (_currentBattle is null) return;
            CampaignMapManager.Instance.SetMapParent();
            EndBattle(_victor);
        }

        public void StartTurn() {
            _currentGangManager.StartTurn();
        }

        public void BattleMenu(CampaignTerritory territory) {
            CampaignUIManager.Instance.SetBattleMenu(territory);
        }

        public void SelectTerritory(CampaignTerritory hoverTerritory) {
            _currentGangManager.SelectTerritory(hoverTerritory);
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

            var battleData = new BattleData {
                Battle = _currentBattle,
                BattleSquadData = new List<(BattleSquad, bool)>()
            };

            foreach (var battleSquad in _currentBattle.BattleBase.Squads) {
                var squadUnit = battleSquad.Units[0].Unit;
                var squad = territory.Squads.Find(sq => sq.Units.Contains(squadUnit));
                var gang = _gangs.Find(g => g.Squads.Contains(squad));
                if (gang is null) battleData.BattleSquadData.Add((battleSquad, false));
                else if (gang.IsPlayerControlled) battleData.BattleSquadData.Add((battleSquad, true));
                else battleData.BattleSquadData.Add((battleSquad, false));
            }
            
            gameObject.AddComponent<BattleStartManager>().SetBattle(battleData);
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
            var territory = Campaign.CampaignMap.Territories.Find(t => t.Squads.Contains(victor));
            var defeated = territory.Squads.Find(s => s != victor);
            var defeatedEntity = CampaignMapManager.Instance.Entities.FirstOrDefault(e => e.Entity == defeated);
            if (defeatedEntity) Destroy(defeatedEntity!.gameObject);
            CampaignMapManager.Instance.Entities.Remove(defeatedEntity);
            territory.Squads.Remove(defeated);
            CampaignUIManager.Instance.SetBattleMenuVictor(victor);
        }
        
        // TODO: Implement actual territory finding
        private CampaignTerritory GetBattleTerritory() => Campaign.CampaignMap.Territories.Find(t => t.Squads.Count > 1);
    }
}