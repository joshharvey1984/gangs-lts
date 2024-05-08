using UnityEngine;

namespace Gangs.Campaign {
    public class CampaignEntityGameObject : MonoBehaviour {
        [SerializeField] private GameObject selectionIndicator;
        public CampaignSquad Entity { get; set; }
        
        public void SetEntity(CampaignSquad entity) {
            Entity = entity;
            entity.OnSelect += Select;
        }

        private void Select(bool select) {
            selectionIndicator.SetActive(select);
        }

        public void Move(CampaignTerritory territory) {
            transform.position = territory.GameObject.transform.position;
        }
    }
}