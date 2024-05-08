using Gangs.Managers;
using TMPro;
using UnityEngine;

namespace Gangs.Campaign.UI {
    public class BattleMenuPanel : MonoBehaviour {
        [SerializeField] private GameObject squadPanelName1;
        [SerializeField] private GameObject squadPanelName2;
        [SerializeField] private GameObject unitPanel1;
        [SerializeField] private GameObject unitPanel2;
        [SerializeField] private GameObject unitPrefab;
        
        [SerializeField] private GameObject autoBattleButton;
        [SerializeField] private GameObject manualBattleButton;
        [SerializeField] private GameObject closeButton;
        
        [SerializeField] private GameObject autoBattlePanel;
        [SerializeField] private GameObject swordAnimation;
        
        public void SetBattleMenu(CampaignTerritory territory) {
            squadPanelName1.GetComponent<TMP_Text>().text = territory.Squads[0].Name;
            squadPanelName2.GetComponent<TMP_Text>().text = territory.Squads[1].Name;
            
            SetUnitPanel(unitPanel1, territory.Squads[0]);
            SetUnitPanel(unitPanel2, territory.Squads[1]);
            
            gameObject.SetActive(true);
        }
        
        public void SetBattleMenuVictor(CampaignSquad victor) {
            Reset();
            var victorName = victor.Name;
            var text = victorName + " is victorious!";
            squadPanelName1.GetComponent<TMP_Text>().text = text;
            squadPanelName2.GetComponent<TMP_Text>().text = text;
            
            SetUnitPanel(unitPanel1, victor);
            
            closeButton.SetActive(true);
            gameObject.SetActive(true);
        }

        private void SetUnitPanel(GameObject panel, CampaignSquad squad) {
            squad.Units.ForEach(u => {
                var unitPanel = Instantiate(unitPrefab, panel.transform);
                unitPanel.GetComponent<BattleMenuUnit>().SetUnit(u);
            });
            
        }
        
        public void AutoBattle() {
            autoBattlePanel.SetActive(true);
            autoBattleButton.SetActive(false);
            manualBattleButton.SetActive(false);
            
            CampaignManager.Instance.AutoBattle();
        }
        
        public void ManualBattle() {
            autoBattleButton.SetActive(false);
            manualBattleButton.SetActive(false);
            CampaignManager.Instance.ManualBattle();
        }

        public void Close() {
            gameObject.SetActive(false);
            autoBattleButton.SetActive(true);
            manualBattleButton.SetActive(true);
            CampaignInputManager.Instance.InputEnabled = true;
            Reset();
        }
        
        private void Reset() {
            squadPanelName1.GetComponent<TMP_Text>().text = "Squad 1";
            squadPanelName2.GetComponent<TMP_Text>().text = "Squad 2";
            
            autoBattlePanel.SetActive(false);
            
            foreach (Transform child in unitPanel1.transform) {
                Destroy(child.gameObject);
            }
            
            foreach (Transform child in unitPanel2.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}
