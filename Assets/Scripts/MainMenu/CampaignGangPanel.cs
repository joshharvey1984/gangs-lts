using Gangs.Data;
using UnityEngine;

namespace Gangs.MainMenu
{
    public class CampaignGangPanel : MonoBehaviour {
        [SerializeField] private GameObject gangNameText;
        [SerializeField] private GameObject clanNameText;
        [SerializeField] private GameObject clanLogoImage;
        [SerializeField] private GameObject playerControlledToggle;
        
        public void SetGang(Gang gang, bool isPlayerControlled) {
            gangNameText.GetComponent<TMPro.TextMeshProUGUI>().text = gang.Name;
            clanNameText.GetComponent<TMPro.TextMeshProUGUI>().text = gang.Faction.Name;
            var texture = gang.Faction.Logo;
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            clanLogoImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            playerControlledToggle.GetComponent<UnityEngine.UI.Toggle>().isOn = isPlayerControlled;
        }
    }
}
