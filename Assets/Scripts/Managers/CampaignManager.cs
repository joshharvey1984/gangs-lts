using System.Collections.Generic;
using Gangs.Campaign;
using Gangs.Data;
using Gangs.MainMenu;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }

        [SerializeField] private GameObject mapParent;
        private CampaignMap _campaignMap;
        
        [SerializeField] private Material tileMaterial;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance is null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        private void Start() {
            var campaignData = new CampaignData {
                CampaignGangs = new List<CampaignGang> {
                    new() { Gang = Gang.All[0], IsPlayerControlled = true },
                    new() { Gang = Gang.All[1] }
                },
                MapSize = CampaignMapSize.Small
            };
            
            _campaignMap = new CampaignMap(campaignData, mapParent);
            CampaignUIManager.Instance.SetCampaignInfo(campaignData);
            CampaignUIManager.Instance.SetTurnNumberText(1);
        }
    }
}