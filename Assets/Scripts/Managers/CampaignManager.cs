using System.Collections.Generic;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }

        [SerializeField] private GameObject mapParent;
        private List<GameObject> _map;
        
        [SerializeField] private Material tileMaterial;

        public int gridSize = 1;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            
            if (Instance is null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        private void Start() {
            
        }
        
        private void Update() {
            
        }
    }
}