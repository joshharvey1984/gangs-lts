using Gangs.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Gangs.Campaign.UI {
    public class CampaignInfoGangIcon : MonoBehaviour {
        [SerializeField] private GameObject turnBox;
        
        public Gang Gang { get; private set; }
        
        public void SetGang(Gang gang) {
            Gang = gang;
            var texture = gang.Clan.Logo;
            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
            GetComponent<Image>().sprite = sprite;
        }
        
        public void SetGangTurn(bool isTurn) {
            turnBox.SetActive(isTurn);
        }

    }
}
