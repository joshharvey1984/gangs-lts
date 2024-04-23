using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }
        
        public Ruleset CurrentRuleset { get; private set; }
        
        public CampaignData CampaignData { get; private set; }
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            DataManager.CreateData();
            CurrentRuleset = Ruleset.All[0];
        }
        
        public void StartCampaign(CampaignData campaignData) {
            CampaignData = campaignData;
            SceneManager.LoadScene("Campaign");
        }
    }
}