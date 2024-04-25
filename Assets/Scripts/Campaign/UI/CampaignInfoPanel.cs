using System.Collections.Generic;
using Gangs.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gangs.Campaign.UI {
    public class CampaignInfoPanel : MonoBehaviour {
        [SerializeField] private GameObject turnText;
        [SerializeField] private GameObject gangPanel;
        
        [SerializeField] private GameObject gangPanelPrefab;
        
        private readonly List<GameObject> _gangPanelInstance = new();
        
        public void AddGangPanel(Gang gang) {
            var gangPanelInstance = Instantiate(gangPanelPrefab, gangPanel.transform);
            gangPanelInstance.GetComponent<CampaignInfoGangIcon>().SetGang(gang);
            _gangPanelInstance.Add(gangPanelInstance);
        }
        
        public void SetTurnNumberText(int turn) {
            turnText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Turn {turn}";
        }
        
        public void SetGangTurn(Gang gang) {
            foreach (var gangIcon in _gangPanelInstance) {
                gangIcon.GetComponent<CampaignInfoGangIcon>()
                    .SetGangTurn(gangIcon.GetComponent<CampaignInfoGangIcon>().Gang == gang);
            }
        }
    }
}
