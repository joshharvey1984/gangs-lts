using System.Collections.Generic;
using Gangs.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers
{
    public class BattleStartManager : MonoBehaviour {
        public static BattleStartManager Instance { get; private set; }
        
        public List<Gang> Gangs { get; set; }
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        public void StartBattle(List<Gang> gangs) {
            Gangs = gangs;
            SceneManager.LoadScene("SampleScene");
        }
    }
}
