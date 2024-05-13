using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class BattleStartManager : MonoBehaviour {
        public static BattleStartManager Instance { get; private set; }
        
        public BattleData BattleData { get; private set; }
        
        private void Awake() {
            if (Instance is not null && Instance != this) Destroy(this); 
            else Instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        
        public void SetBattle(BattleData battle) {
            BattleData = battle;
            BattleAI.BattleBase = BattleData.Battle.BattleBase;
            SceneManager.LoadScene("Battle");
        }
    }
    
    public struct BattleData {
        public IBattle Battle;
        public List<(BattleSquad Squad, bool PlayerControlled)> BattleSquadData;
        
        public bool PlayerControllerUnit(BattleUnit unit) => BattleSquadData.Any(s => s.Squad.Units.Contains(unit) && s.PlayerControlled);
    }
}