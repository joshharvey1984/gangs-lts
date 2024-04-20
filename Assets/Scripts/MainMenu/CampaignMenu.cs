using System.Collections.Generic;
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
                CampaignGangs = new List<CampaignGang>()
            };

            AddGang(Gang.All[0], true);
            AddGang(Gang.All[1]);
        }
        
        public void StartGame() {
            GameManager.Instance.StartCampaign(_campaignData);
        }

        private void AddGang(Gang gang, bool isPlayerControlled = false) {
            _campaignData.CampaignGangs.Add(new CampaignGang { Gang = gang, IsPlayerControlled = isPlayerControlled });
            var gangObject = Instantiate(gangPrefab, gangListPanel.transform);
            gangObject.GetComponent<CampaignGangPanel>().SetGang(gang, isPlayerControlled);
        }
    }
    
    public struct CampaignData {
        public List<CampaignGang> CampaignGangs;
        public CampaignMapSize MapSize;
    }
    
    public struct CampaignGang {
        public Gang Gang;
        public bool IsPlayerControlled;
    }
    
    public enum CampaignMapSize {
        Small,
        Medium,
        Large
    }
    
    public static class CampaingMapExtensions {
        public static int GetGridSize(this CampaignMapSize mapSize) {
            return mapSize switch {
                CampaignMapSize.Small => 4,
                CampaignMapSize.Medium => 9,
                CampaignMapSize.Large => 16,
                _ => 1
            };
        }
    }
}
