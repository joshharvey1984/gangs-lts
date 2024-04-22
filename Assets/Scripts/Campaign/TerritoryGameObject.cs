using UnityEngine;
using UnityEngine.ProBuilder;

namespace Gangs.Campaign
{
    public class TerritoryGameObject : MonoBehaviour {
        [SerializeField] private LineRenderer borderRenderer;
        private void Awake() {
            borderRenderer = GetComponent<LineRenderer>();
        }
        
        public void CreateBorder() {
            var pbMesh = GetComponent<ProBuilderMesh>();
            if (pbMesh is null) return;
            
            var vertices = pbMesh.positions;
            borderRenderer.positionCount = vertices.Count + 1; // +1 to close the loop
            for (var i = 0; i < vertices.Count; i++) {
                borderRenderer.SetPosition(i, transform.TransformPoint(vertices[i]));
            }
            borderRenderer.SetPosition(vertices.Count, transform.TransformPoint(vertices[0]));

            borderRenderer.startWidth = 0.1f;
            borderRenderer.endWidth = 0.1f;
            borderRenderer.material = new Material(Shader.Find("Sprites/Default")) {
                color = Color.black
            };
        }
        
        public void SetBorderColour(Color colour) {
            borderRenderer.material.color = colour;
        }
    }
}
