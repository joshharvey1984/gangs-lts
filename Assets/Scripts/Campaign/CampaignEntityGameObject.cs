using UnityEngine;

namespace Gangs.Campaign {
    public class CampaignEntityGameObject : MonoBehaviour {
        [SerializeField] private GameObject selectionIndicator;
        
        public void Select() {
            selectionIndicator.SetActive(true);
        }

        public void Move(CampaignTerritory territory) {
            transform.position = territory.GameObject.transform.position;
        }
    }
}