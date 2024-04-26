using Gangs.Campaign;
using Gangs.Campaign.UI;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignUIManager : MonoBehaviour {
        public static CampaignUIManager Instance { get; private set; }
        
        [SerializeField] private GameObject territoryInfoPanel;
        [SerializeField] private GameObject campaignInfoPanel;
        [SerializeField] private GameObject battleMenu;
        
        private void Awake() {
            if (Instance is null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        public void SetCampaignInfo(CampaignData campaign) {
            campaign.CampaignGangs.ForEach(c => campaignInfoPanel.GetComponent<CampaignInfoPanel>().AddGangPanel(c.BaseGang));
        }
        
        public void SetTurnNumberText(int turn) {
            campaignInfoPanel.GetComponent<CampaignInfoPanel>().SetTurnNumberText(turn);
        }

        public void SetTerritoryInfo(CampaignTerritory territory) {
            territoryInfoPanel.GetComponent<TerritoryInfoPanel>().SetTerritory(territory);
        }

        public void SetGangTurn(Gang gang) {
            campaignInfoPanel.GetComponent<CampaignInfoPanel>().SetGangTurn(gang);
        }

        public void SetBattleMenu(CampaignTerritory territory) {
            battleMenu.GetComponent<BattleMenuPanel>().SetBattleMenu(territory);
        }
    }
}