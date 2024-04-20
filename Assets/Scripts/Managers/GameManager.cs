using Gangs.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs.Managers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }
        
        public CampaignData CampaignData { get; private set; }
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public void StartCampaign(CampaignData campaignData) {
            CampaignData = campaignData;
            SceneManager.LoadScene("Campaign");
        }
    }
}