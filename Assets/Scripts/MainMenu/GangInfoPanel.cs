using Gangs.Data;
using UnityEngine;

namespace Gangs.MainMenu
{
    public class GangInfoPanel : MonoBehaviour {
        [SerializeField] private GameObject gangeName;
        [SerializeField] private GameObject clanName;
        [SerializeField] private GameObject clanLogo;
        
        public void SetGang(Gang gang) {
            gangeName.GetComponent<TMPro.TextMeshProUGUI>().text = gang.Name;
            clanName.GetComponent<TMPro.TextMeshProUGUI>().text = gang.Clan.Name;
            
            //conver texture to sprite
            var texture = gang.Clan.Logo;
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            clanLogo.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }

    }
}
