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
        
        public void SetBattleMenu(CampaignTerritory territory) {
            squadPanelName1.GetComponent<TMP_Text>().text = territory.Squads[0].Name;
            squadPanelName2.GetComponent<TMP_Text>().text = territory.Squads[1].Name;
            
            SetUnitPanel(unitPanel1, territory.Squads[0]);
            SetUnitPanel(unitPanel2, territory.Squads[1]);
            
            gameObject.SetActive(true);
        }
        
        private void SetUnitPanel(GameObject panel, CampaignSquad squad) {
            squad.Units.ForEach(u => {
                var unitPanel = Instantiate(unitPrefab, panel.transform);
                unitPanel.GetComponent<BattleMenuUnit>().SetUnit(u);
            });
            
        }
        
        public void AutoBattle() {
            gameObject.SetActive(false);
            CampaignManager.Instance.Battle();
        }
        
        public void ManualBattle() {
            gameObject.SetActive(false);
        }
    }
}
