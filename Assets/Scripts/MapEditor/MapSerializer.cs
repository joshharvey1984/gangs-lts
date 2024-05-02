using System;
using System.Collections.Generic;
using System.IO;
using Gangs.Core;
using UnityEditor;
using UnityEngine;

namespace Gangs.MapEditor
{
    public class MapSerializer : MonoBehaviour {
        public GameObject tilePrefab;
        public GameObject wallPrefab;
        public GameObject halfWallPrefab;
        public GameObject ladderPrefab;
        
        public void Save() {
            var map = new Grid {
                tiles = new List<Tile>(),
                walls = new List<Wall>(),
                ladders = new List<Ladder>()
            };

            foreach (var tile in GameObject.FindGameObjectsWithTag(nameof(Tile))) {
                map.tiles.Add(new Tile(tile.transform.position));
            }

            foreach (var wall in GameObject.FindGameObjectsWithTag(nameof(Wall))) {
                var cover = wall.GetComponent<MapWallData>().coverType;
                var lineOfSightBlocker = wall.GetComponent<MapWallData>().lineOfSightBlocker;
                map.walls.Add(new Wall(wall.transform.position, cover, lineOfSightBlocker));
            }
            
            foreach (var ladder in GameObject.FindGameObjectsWithTag(nameof(Ladder))) {
                map.ladders.Add(new Ladder(ladder.transform));
            }

            var json = JsonUtility.ToJson(map);
            var path = Application.dataPath + "/Data/Maps/Test/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".json";
            File.WriteAllText(path, json);
            
            Debug.Log("Map saved to " + path);
        }

        public void Load() {
            var path = Application.dataPath + "/Data/Maps/Test/";
            var file = EditorUtility.OpenFilePanel("Load map", path, "json");
            if (file.Length == 0) return;
            
            var json = File.ReadAllText(file);
            var map = JsonUtility.FromJson<Grid>(json);
            
            foreach (var tile in map.tiles) {
                var newTile = Instantiate(tilePrefab, new Vector3(tile.x, tile.y, tile.z), Quaternion.identity, transform);
                newTile.tag = nameof(Tile);
            }
            
            foreach (var wall in map.walls) {
                if (wall.coverType == CoverType.Full) {
                    var newWall = Instantiate(wallPrefab, new Vector3(wall.x, wall.y, wall.z), Quaternion.identity, transform);
                    newWall.tag = nameof(Wall);
                }
                else {
                    var newWall = Instantiate(halfWallPrefab, new Vector3(wall.x, wall.y, wall.z), Quaternion.identity, transform);
                    newWall.tag = nameof(Wall);
                }
            }
            
            foreach (var ladder in map.ladders) {
                var newLadder = Instantiate(ladderPrefab, new Vector3(ladder.x, ladder.y, ladder.z), Quaternion.Euler(0, ladder.rotation, 0), transform);
                newLadder.tag = nameof(Ladder);
            }
        }
        
        public void Clear() {
            foreach (var tile in GameObject.FindGameObjectsWithTag(nameof(Tile))) {
                Destroy(tile);
            }
            
            foreach (var wall in GameObject.FindGameObjectsWithTag(nameof(Wall))) {
                Destroy(wall);
            }
            
            foreach (var ladder in GameObject.FindGameObjectsWithTag(nameof(Ladder))) {
                Destroy(ladder);
            }
        }
    }
        
    [Serializable]
    public struct Grid {
        public List<Tile> tiles;
        public List<Wall> walls;
        public List<Ladder> ladders;
    }
    
    [Serializable]
    public struct Tile {
        public int x;
        public int y;
        public int z;
        
        public Tile(Vector3 position) {
            x = (int) position.x;
            y = (int) position.y;
            z = (int) position.z;
        }
    }

    [Serializable]
    public struct Wall {
        public float x;
        public int y;
        public float z;
        public CoverType coverType;
        public bool lineOfSightBlocker;
        
        public Wall(Vector3 position, CoverType coverType, bool lineOfSightBlocker = true) {
            x = position.x;
            y = (int) position.y;
            z = position.z;
            this.coverType = coverType;
            this.lineOfSightBlocker = lineOfSightBlocker;
        }
    }
    
    [Serializable]
    public struct Ladder {
        public float x;
        public int y;
        public float z;
        public int rotation;
        
        public Ladder(Transform transform) {
            x = transform.position.x;
            y = (int) transform.position.y;
            z = transform.position.z;
            rotation = (int) transform.rotation.eulerAngles.y;
        }
    }
}

    
