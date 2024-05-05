using Gangs.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers
{
    public class BattleStartManager : MonoBehaviour {
        public static BattleStartManager Instance { get; private set; }
        
        public IBattle Battle { get; private set; }
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        public void SetBattle(IBattle battle) {
            Battle = battle;
            SceneManager.LoadScene("Battle");
        }
    }
}