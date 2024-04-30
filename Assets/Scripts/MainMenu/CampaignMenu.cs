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
                CampaignGangManagers = new List<CampaignGangManager>() {
                    
                },
                MapSize = CampaignMapSize.Small
            };

            
        }
        
        public void StartGame() {
            GameManager.Instance.StartCampaign(_campaignData);
        }

        // private void AddGang(Faction faction, bool isPlayerControlled = false) {
        //     _campaignData.CampaignFactions.Add(new Faction.All[0], IsPlayerControlled = isPlayerControlled });
        //     var gangObject = Instantiate(gangPrefab, gangListPanel.transform);
        //     gangObject.GetComponent<CampaignGangPanel>().SetGang(gang, isPlayerControlled);
        // }
    }
    
    public struct CampaignData {
        public List<CampaignGangManager> CampaignGangManagers;
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
