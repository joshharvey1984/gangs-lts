using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Campaign.GameObjects
{
    public class CampaignMapGameObject : MonoBehaviour {
        private List<GameObject> _territories;
        private void Awake() {
            _territories = new List<GameObject>();
            _territories.AddRange(gameObject.GetComponentsInChildren<Transform>().Where(t => t != transform).Select(t => t.gameObject));
        }
        
        private void Start() {
            var middle = new Vector3();
            middle = _territories.Aggregate(middle, (current, tile) => current + tile.transform.position);
            middle /= _territories.Count;
            
            foreach (var t in _territories) {
                var distance = Vector3.Distance(middle, t.transform.position);
                t.transform.position += (t.transform.position - middle).normalized * distance * 0.1f;
            }
            
            foreach (var territory in _territories) {
                territory.GetComponent<CampaignTerritoryGameObject>().CreateBorder();
            }

            CampaignManager.Instance.LoadedMap();
        }
        
        public List<GameObject> GetActiveTerritories(int gridSize) {
            var centralTerritories = new List<GameObject>();
            var middle = new Vector3();
            middle = _territories.Aggregate(middle, (current, tile) => current + tile.transform.position);
            middle /= _territories.Count;
            
            var sortedTerritories = _territories.OrderBy(territory => Vector3.Distance(territory.transform.position, middle)).ToList();
            centralTerritories.AddRange(sortedTerritories.Take(gridSize));
            
            // deactavate the rest
            foreach (var territory in sortedTerritories.Skip(gridSize)) {
                territory.GetComponent<CampaignTerritoryGameObject>().Deactivate();
            }
            
            return centralTerritories;
        }
        
        public List<GameObject> FindSpawnPoints(int numGangs, List<CampaignTerritoryGameObject> validTerritories = null) {
            var spawnPoints = new List<GameObject>();
            var activeTerritories = _territories.Where(t => t.GetComponent<CampaignTerritoryGameObject>().Active).ToList();
            if (validTerritories != null) {
                activeTerritories = activeTerritories.Where(t => validTerritories.Contains(t.GetComponent<CampaignTerritoryGameObject>())).ToList();
            }
    
            if (activeTerritories.Count < numGangs)
                //error
                throw new DataException("Not enough territories to spawn gangs");

            // Find the pair of territories furthest apart
            GameObject territory1 = null;
            GameObject territory2 = null;
            float maxDistance = 0f;

            for (int i = 0; i < activeTerritories.Count - 1; i++)
            {
                for (int j = i + 1; j < activeTerritories.Count; j++)
                {
                    float distance = Vector3.Distance(activeTerritories[i].transform.position, activeTerritories[j].transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        territory1 = activeTerritories[i];
                        territory2 = activeTerritories[j];
                    }
                }
            }

            spawnPoints.Add(territory1);
            spawnPoints.Add(territory2);

            for (int i = 2; i < numGangs; i++)
            {
                GameObject furthestTerritory = null;
                maxDistance = 0f;

                foreach (var territory in activeTerritories)
                {
                    if (spawnPoints.Contains(territory))
                        continue;

                    float minDistance = float.MaxValue;

                    foreach (var spawnPoint in spawnPoints)
                    {
                        float distance = Vector3.Distance(territory.transform.position, spawnPoint.transform.position);
                        minDistance = Mathf.Min(minDistance, distance);
                    }

                    if (minDistance > maxDistance)
                    {
                        maxDistance = minDistance;
                        furthestTerritory = territory;
                    }
                }

                if (furthestTerritory != null)
                    spawnPoints.Add(furthestTerritory);
            }
    
            return spawnPoints;
        }
    }
}
