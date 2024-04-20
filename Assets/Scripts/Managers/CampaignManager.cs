using System.Collections.Generic;
using Gangs.Campaign.MapGenerator;
using UnityEngine;

namespace Gangs.Managers {
    public class CampaignManager : MonoBehaviour {
        public static CampaignManager Instance { get; private set; }

        [SerializeField] private GameObject mapParent;
        public List<GameObject> map = new();
        
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
            map = CampaignMapGenerator.GenerateMap(gridSize, tileMaterial);
            map.ForEach(tile => tile.transform.SetParent(mapParent.transform));
        }
        
        private void Update() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                var tempMap = new List<GameObject>(map);
                tempMap.ForEach(Destroy);
                map.Clear();
                map = CampaignMapGenerator.GenerateMap(gridSize, tileMaterial);
                map.ForEach(tile => tile.transform.SetParent(mapParent.transform));
            }
        }
    }
}