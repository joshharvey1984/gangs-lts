using System;
using Gangs.Campaign;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignInputManager : MonoBehaviour {
        public static CampaignInputManager Instance { get; private set; }
        
        public CampaignTerritory HoverTerritory { get; private set; }
        
        public event Action<CampaignTerritory> OnLeftClickTerritory; 
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                HandleLeftClick();
            }
            
            if (Input.GetMouseButtonDown(1)) {
                HandleRightClick();
            }
            
            CheckHoverTerritory();
        }

        private void CheckHoverTerritory() {
            var territory = GetMouseTerritory();
            if (territory is null) return;
            HoverTerritory = territory;
        }

        private CampaignTerritory GetMouseTerritory() {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return null;
            if (!hit.collider.CompareTag("Territory")) return null;
            var coll = hit.collider.GetComponent<CampaignTerritoryGameObject>();
            return CampaignManager.Instance.GetTerritory(coll);
        }

        public void SelectSquad(CampaignSquad squad) {
            var territory = CampaignManager.Instance.GetTerritory(squad);
            territory.Neighbours.ForEach(t => t.GameObject.Highlight(Color.yellow));
        }
        
        public static void DeselectSquad() {
            CampaignManager.Instance.ResetTerritoryHighlights();
        }
        
        private void HandleLeftClick() {
            if (HoverTerritory is null) return;
            OnLeftClickTerritory?.Invoke(HoverTerritory);
        }
        
        private void HandleRightClick() {
        }
    }
}