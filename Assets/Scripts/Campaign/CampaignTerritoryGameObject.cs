using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Gangs.Campaign
{
    public class CampaignTerritoryGameObject : MonoBehaviour {
        [SerializeField] private LineRenderer borderRenderer;
        [SerializeField] private GameObject entityPrefab;
        public bool Active { get; set; }
        
        public event Action OnMouseEnterEvent;
        public event Action OnMouseExitEvent;
        
        private Color _colour = Color.black;

        private void Awake() {
            borderRenderer = GetComponent<LineRenderer>();
        }

        private void OnMouseEnter() {
            if (!Active) return;
            SetBorderColour(Color.white);
            OnMouseEnterEvent?.Invoke();
        }
        
        private void OnMouseExit() {
            if (!Active) return;
            SetBorderColour(_colour);
            OnMouseExitEvent?.Invoke();
        }

        public void CreateBorder() {
            var pbMesh = GetComponent<ProBuilderMesh>();
            if (pbMesh is null) return;
            
            var vertices = pbMesh.positions;
            borderRenderer.positionCount = vertices.Count + 1;
            for (var i = 0; i < vertices.Count; i++) {
                borderRenderer.SetPosition(i, transform.TransformPoint(vertices[i]));
            }
            borderRenderer.SetPosition(vertices.Count, transform.TransformPoint(vertices[0]));

            borderRenderer.startWidth = 0.1f;
            borderRenderer.endWidth = 0.1f;
            borderRenderer.material = new Material(Shader.Find("Sprites/Default")) {
                color = _colour
            };
        }
        
        public void SetBorderColour(Color colour) {
            borderRenderer.material.color = colour;
        }
        
        public void Deactivate() {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
            Active = false;
        }
        
        public void SetColour(Color colour) {
            gameObject.GetComponent<MeshRenderer>().material.color = colour;
        }
        
        public List<CampaignTerritoryGameObject> FindNeighbours() {
            var neighbours = new List<CampaignTerritoryGameObject>();
    
            var raycastDirections = 36;
            var maxDistance = 3f;

            for (var i = 0; i < raycastDirections; i++) {
                var angle = i * (360f / raycastDirections);
                var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

                if (!Physics.Raycast(transform.position, direction, out var hitInfo, maxDistance)) continue;
                if (hitInfo.collider.gameObject == gameObject) continue;
                var neighbour = hitInfo.collider.gameObject.GetComponent<CampaignTerritoryGameObject>();
                if (neighbour == null || neighbours.Contains(neighbour)) continue;
                if (neighbour.Active) neighbours.Add(neighbour);
            }
            
            return neighbours;
        }

        public CampaignEntityGameObject SpawnEntity(CampaignSquad campaignEntity) {
            var entity = Instantiate(entityPrefab, transform.position, Quaternion.identity);
            entity.name = campaignEntity.Name;
            entity.GetComponent<CampaignEntityGameObject>().SetEntity(campaignEntity);
            return entity.GetComponent<CampaignEntityGameObject>();
        }

        public void Highlight(Color color) {
            _colour = color;
        }

        public void ResetColour() {
            _colour = Color.black;
            SetBorderColour(_colour);
        }
    }
}
