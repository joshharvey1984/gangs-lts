using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gangs.Campaign.MapGenerator {
    public static class CampaignMapGenerator {
        private static int[,] _mapArray = new int[100, 100];
        private static int _territoryCount = 0;
        
        public static List<GameObject> GenerateMap(int gridSize, Material material) {
            var map = new List<GameObject>();

            _mapArray = new int[100, 100];
            for (var x = 0; x < 100; x++) {
                for (var y = 0; y < 100; y++) {
                    _mapArray[x, y] = -1;
                }
            }
            
            ThreeTerritoryChunk((0, 0));
            ThreeTerritoryChunk((16, 0));
            ThreeTerritoryChunk((0, 20));
            ThreeTerritoryChunk((16, 20));
            
            map = PlaceTerritoryMeshes(material);
            
            return map;
        }

        private static void ThreeTerritoryChunk((int X, int Y) start) {
            var width = 8;
            var height = 12;
            
            PlaceIntTerritory(start, (width, height));
            PlaceIntTerritory((start.X + width, start.Y), (width, height));
            InvertTerritory((start.X + width - 1, start.X + width - 1), (start.Y, height));
            PlaceIntTerritory((start.X, start.Y + height), (width * 2, width));
            InvertTerritory((start.X, start.X + (width * 2) - 1), (start.Y + height - 1, start.Y +  height - 1));
        }
        
        private static void InvertTerritory((int, int) startEndX, (int, int) startEndY) {
            var invert = false;
            for (var x = startEndX.Item1; x <= startEndX.Item2; x++) {
                for (var y = startEndY.Item1; y <= startEndY.Item2; y++) {
                    if (!invert)
                        if (Random.Range(0, 100) < 10) invert = true;
                    if (invert) {
                        _mapArray[x, y] = _territoryCount - 1;
                    }
                }
            }
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
                    mesh.name = _mapArray[x, y].ToString();
                    territoryMeshes.Add(mesh);
                }
            }

            // Join meshes based on name
            for (var i = 0; i < _territoryCount; i++)
            {
                var territory = new GameObject($"Territory {i}");
                territory.transform.localScale = new Vector3(1, 1, 1);
                territory.SetActive(false);
                territory.AddComponent<MeshFilter>();
                territory.AddComponent<MeshRenderer>();
                var combinedMesh = new Mesh();

                var meshesToCombine = territoryMeshes.FindAll(mesh => mesh.name == i.ToString());
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
                territory.SetActive(true);

                var he = territory.AddComponent<HighlightEffect>();
                he.highlighted = true;
                
                tiles.Add(territory);
            }

            return tiles;
        }
    }
}