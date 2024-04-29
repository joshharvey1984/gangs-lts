using System.Collections.Generic;
using Gangs.Campaign;
using Gangs.Data;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.MainMenu {
    public class CampaignMenu : MonoBehaviour {
        [SerializeField] private GameObject gangPrefab;
        [SerializeField] private GameObject gangListPanel;
        [SerializeField] private GameObject startButton;
        
        private CampaignData _campaignData;
        
        private void OnEnable() {
            _campaignData = new CampaignData {
                CampaignGangs = new List<CampaignGangManager>()
            };

            AddGang(Gang.All[0], true);
            AddGang(Gang.All[1]);
        }
        
        public void StartGame() {
            GameManager.Instance.StartCampaign(_campaignData);
        }

        private void AddGang(Gang gang, bool isPlayerControlled = false) {
            _campaignData.CampaignGangs.Add(new CampaignGangManager { BaseGang = gang, IsPlayerControlled = isPlayerControlled });
            var gangObject = Instantiate(gangPrefab, gangListPanel.transform);
            gangObject.GetComponent<CampaignGangPanel>().SetGang(gang, isPlayerControlled);
        }
    }
    
    public struct CampaignData {
        public List<CampaignGangManager> CampaignGangs;
        public CampaignMapSize MapSize;
    }
    
    public enum CampaignMapSize {
        Small,
        Medium,
        Large
    }
    
    public static class CampaignMapExtensions {
        public static int GetGridSize(this CampaignMapSize mapSize) {
            return mapSize switch {
                CampaignMapSize.Small => 12,
                CampaignMapSize.Medium => 18,
                CampaignMapSize.Large => 27,
                _ => 1
            };
        }
    }
}
