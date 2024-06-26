﻿using System;
using Gangs.Campaign;
using Gangs.Campaign.GameObjects;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignInputManager : MonoBehaviour {
        public static CampaignInputManager Instance { get; private set; }
        
        public bool InputEnabled { get; set; } = true;
        public CampaignTerritory HoverTerritory { get; private set; }
        
        public event Action<CampaignTerritory> OnLeftClickTerritory; 
        
        private void Awake() {
            if (Instance is not null && Instance != this) Destroy(this); 
            else Instance = this;
        }
        
        private void Update() {
            if (!InputEnabled) return;
            
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
            return CampaignMapManager.Instance.Territories
        }

        public void SelectSquad(CampaignSquad squad) {
            var territory = CampaignMapManager.Instance.GetTerritory(squad);
            territory.Neighbours.ForEach(t => t.GameObject.Highlight(Color.yellow));
        }
        
        public static void DeselectSquad() {
            CampaignMapManager.Instance.ResetTerritoryHighlights();
        }
        
        private void HandleLeftClick() {
            if (HoverTerritory is null) return;
            CampaignManager.Instance.SelectTerritory(HoverTerritory);
        }
        
        private void HandleRightClick() {
        }
    }
}