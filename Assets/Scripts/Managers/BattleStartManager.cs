using Gangs.Battle;
using Gangs.Battle.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class BattleStartManager : MonoBehaviour {
        public static BattleStartManager Instance { get; private set; }
        
        public IBattle Battle { get; private set; }
        
        private void Awake() {
            if (Instance is not null && Instance != this) Destroy(this); 
            else Instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        
        public void SetBattle(IBattle battle) {
            Battle = battle;
            BattleAI.BattleBase = Battle.BattleBase;
            SceneManager.LoadScene("Battle");
        }
    }
}