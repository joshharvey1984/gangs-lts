using Gangs.Managers;
using UnityEngine;

namespace Gangs.MainMenu {
    public class MainMenu : MonoBehaviour {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject skirmishPanel;
        [SerializeField] private GameObject campaignPanel;
        
        private void Awake() {
            skirmishPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            DataManager.CreateData();
        }
        
        public void NewCampaign() {
            mainMenuPanel.SetActive(false);
            campaignPanel.SetActive(true);
        }

        public void NewSkirmish() {
            mainMenuPanel.SetActive(false);
            skirmishPanel.SetActive(true);
        }
        
        public void MainMenuButton() {
            skirmishPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}
