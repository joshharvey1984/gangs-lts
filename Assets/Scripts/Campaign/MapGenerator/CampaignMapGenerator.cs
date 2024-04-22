using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gangs.Campaign.MapGenerator {
    public static class CampaignMapGenerator {
        private static int[,] _mapArray = new int[100, 100];
        private static int _territoryCount;
        
        public static List<GameObject> GenerateMap(int gridSize, Material material) {
            _mapArray = new int[100, 100];
            _territoryCount = 0;
            
            for (var x = 0; x < 100; x++) {
                for (var y = 0; y < 100; y++) {
                    _mapArray[x, y] = -1;
                }
            }
            
            for (var i = 0; i < 20; i++) {
                var width = Random.Range(6, 14);
                var height = Random.Range(6, 14);
                
                if (Random.Range(0, 100) < 33) {
                    (width, height) = (height, width);
                }
                
                var leeway = Random.Range(0, 3);
                var closestEmptyToCenter = FindClosestEmptyToCentreThatWillFit(width, height, leeway);
                PlaceIntTerritory(closestEmptyToCenter, (width, height));
            }
            
            // finally we spiral out from the centre and fill in any remaining gaps
            FillEmptySurroundedPositions(2);
            FillEmptySurroundedPositions(4);
            
            var t = PlaceTerritoryMeshes(material);
            //
            foreach (var territories in t) {
                // use territory.name = $"{meshPos.x},{meshPos.z}";
                var territoryPosition = territories.name.Split(',').Select(float.Parse).ToArray();
                var v3 = new Vector3(territoryPosition[0], 0, territoryPosition[1]);
                var distanceFromCentre = Mathf.Abs(v3.x - 50) + Mathf.Abs(v3.z - 50);
                var direction = new Vector3(v3.x - 50, 0, v3.z - 50).normalized;
                var newPos = v3 + direction * (distanceFromCentre * 0.1f);
                var bounds = territories.GetComponent<MeshFilter>().mesh.bounds;
                bounds.center = newPos;
                territories.GetComponent<MeshFilter>().mesh.bounds = bounds;
            }
            
            return t;
        }
        
        private static void FillEmptySurroundedPositions(int numSides) {
            var emptySurroundedPositions = FindEmptySurroundedPositions(numSides);
            foreach (var position in emptySurroundedPositions) {
                var closestTerritory = FindClosestTerritory(position.Item1, position.Item2);
                _mapArray[position.Item1, position.Item2] = closestTerritory;
            }
        }

        private static (int, int)[] FindEmptySurroundedPositions(int numSides) {
            // find all empty positions that have two sides touching a territory
            var emptyPositions = new List<(int, int)>();
            for (var x = 0; x < 100; x++) {
                for (var y = 0; y < 100; y++) {
                    if (_mapArray[x, y] != -1) continue;
                    if (x > 0 && x < 99 && y > 0 && y < 99) {
                        var sideCount = 0;
                        if (_mapArray[x - 1, y] != -1) sideCount++;
                        if (_mapArray[x + 1, y] != -1) sideCount++;
                        if (_mapArray[x, y - 1] != -1) sideCount++;
                        if (_mapArray[x, y + 1] != -1) sideCount++;
                        if (sideCount >= numSides) {
                            emptyPositions.Add((x, y));
                        }
                    }
                }
            }
            
            return emptyPositions.ToArray();
        }
        
        private static int FindClosestTerritory(int x, int y) {
            var closest = 0;
            var closestDistance = 1000;
            for (var i = 0; i < _territoryCount; i++) {
                for (var j = 0; j < 100; j++) {
                    for (var k = 0; k < 100; k++) {
                        if (_mapArray[j, k] == i) {
                            var distance = Mathf.Abs(j - x) + Mathf.Abs(k - y);
                            if (distance < closestDistance) {
                                closest = i;
                                closestDistance = distance;
                            }
                        }
                    }
                }
            }

            return closest;
        }
        
        private static (int, int) FindClosestEmptyToCentreThatWillFit(int width, int height, int leeway = 0) {
            var closest = (0, 0);
            var closestDistance = 1000;
            for (var x = 0; x < 100; x++) {
                for (var y = 0; y < 100; y++) {
                    if (_mapArray[x, y] != -1) continue;
                    var distance = Mathf.Abs(x - 50) + Mathf.Abs(y - 50);
                    if (distance < closestDistance && CanFitBlock(x, y, width, height, leeway)) {
                        closest = (x, y);
                        closestDistance = distance;
                    }
                }
            }

            return closest;
        }

        private static bool CanFitBlock(int startX, int startY, int width, int height, int leeway = 0) {
            if (startX + width - leeway > 100 || startY + height - leeway > 100) {
                return false;
            }

            for (var x = startX; x < startX + width - leeway; x++) {
                for (var y = startY; y < startY + height - leeway; y++) {
                    if (_mapArray[x, y] != -1) {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private static void PlaceIntTerritory((int, int) position, (int, int) size) {
            for (var x = position.Item1; x < position.Item1 + size.Item1; x++) {
                for (var y = position.Item2; y < position.Item2 + size.Item2; y++) {
                    _mapArray[x, y] = _territoryCount;
                }
            }
            
            _territoryCount++;
        }
        
        private static List<GameObject> PlaceTerritoryMeshes(Material material = null) {
            var tiles = new List<GameObject>();
            var territoryMeshes = new List<Mesh>();
            
            for (var x = 0; x < 100; x++) {
                for (var y = 0; y < 100; y++) {
                    if (_mapArray[x, y] == -1) continue;
                    var mesh = new Mesh();
                    var vertices = new Vector3[4];
                    vertices[0] = new Vector3(x, 0, y);
                    vertices[1] = new Vector3(x + 1, 0, y);
                    vertices[2] = new Vector3(x, 0, y + 1);
                    vertices[3] = new Vector3(x + 1, 0, y + 1);
                    mesh.vertices = vertices;
                    mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();
                    mesh.name = $"{_mapArray[x, y].ToString()}";
                    territoryMeshes.Add(mesh);
                }
            }

            // Join meshes based on name
            for (var i = 0; i < _territoryCount; i++) {
                var territory = new GameObject();
                territory.AddComponent<MeshFilter>();
                territory.AddComponent<MeshRenderer>();
                var combinedMesh = new Mesh();

                var meshesToCombine = territoryMeshes.FindAll(mesh => mesh.name == $"{i}");
                var combineInstances = new CombineInstance[meshesToCombine.Count];
                
                for (var j = 0; j < meshesToCombine.Count; j++)
                {
                    combineInstances[j].mesh = meshesToCombine[j];
                    combineInstances[j].transform = territory.transform.localToWorldMatrix;
                }
                
                combinedMesh.CombineMeshes(combineInstances);
                combinedMesh.RecalculateNormals();
                combinedMesh.RecalculateBounds();
                
                territory.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
                territory.GetComponent<MeshRenderer>().material = material;
                
                var meshPos = combinedMesh.bounds.center;
                territory.name = $"{meshPos.x},{meshPos.z}";
                
                tiles.Add(territory);
            }

            return tiles;
        }
    }
}