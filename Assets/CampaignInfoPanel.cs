using Gangs.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gangs {
    public class CampaignInfoPanel : MonoBehaviour {
        [SerializeField] private GameObject turnText;
        [SerializeField] private GameObject gangPanel;
        
        [SerializeField] private GameObject gangPanelPrefab;
        
        public void AddGangPanel(Gang gang) {
            var gangPanelInstance = Instantiate(gangPanelPrefab, gangPanel.transform);
            var texture = gang.Clan.Logo;
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            gangPanelInstance.GetComponent<Image>().sprite = sprite;
        }
        
        public void SetTurnNumberText(int turn) {
            turnText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Turn {turn}";
        }
    }
}
