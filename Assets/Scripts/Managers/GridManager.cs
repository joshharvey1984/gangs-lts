using System;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class GridManager : MonoBehaviour {
        public static GridManager Instance;
        public Grid.Grid Grid { get; private set; }
        
        public void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
        }
        
        public Grid.Grid SetupGrid() {
            SetupTiles();
            SetupProps();
            SetupWalls();
            SetupLadders();
            return Grid;
        }

        private void SetupTiles() {
            var tiles = GameObject.FindGameObjectsWithTag($"Tile");
            
            var maxX = 0;
            var maxY = 0;
            var maxZ = 0;
            foreach (var tile in tiles) {
                var tilePosition = tile.transform.position;
                if (tilePosition.x > maxX) {
                    maxX = (int)tilePosition.x;
                }
                if (tilePosition.y > maxY) {
                    maxY = (int)tilePosition.y;
                }
                if (tilePosition.z > maxZ) {
                    maxZ = (int)tilePosition.z;
                }
            }
            
            Grid = new Grid.Grid(maxX + 1, maxY + 1, maxZ + 1);
            
            foreach (var tile in tiles) {
                var position = tile.transform.position;
                Grid.AddTile(new GridPosition((int)position.x, (int)position.y, (int)position.z));
            }
        }

        private void SetupProps() {
            var props = GameObject.FindGameObjectsWithTag($"Prop");
            foreach (var prop in props) {
                var propPosition = prop.transform.position;
                Grid.AddProp(new GridPosition((int)propPosition.x, (int)propPosition.y, (int)propPosition.z));
            }
        }

        private void SetupLadders() {
            var ladders = GameObject.FindGameObjectsWithTag($"Ladder");
            foreach (var ladder in ladders) {
                var ladderPosition = ladder.transform.position;
                if (ladderPosition.x % 2 == 0) {
                    Grid.AddLadder(new GridPosition((int)ladderPosition.x, (int)ladderPosition.y, Mathf.RoundToInt(ladderPosition.z - 0.4f)), 
                        new GridPosition((int)ladderPosition.x, (int)ladderPosition.y + 1, Mathf.RoundToInt(ladderPosition.z + 0.6f)));
                }
                else {
                    Grid.AddLadder(new GridPosition(Mathf.RoundToInt(ladderPosition.x - 0.4f), (int)ladderPosition.y, (int)ladderPosition.z), 
                        new GridPosition(Mathf.RoundToInt(ladderPosition.x + 0.6f), (int)ladderPosition.y + 1, (int)ladderPosition.z));
                }
            }
        }
        
        private void SetupWalls() {
            var walls = GameObject.FindGameObjectsWithTag($"Wall");
            foreach (var wall in walls) {
                var wallPosition = wall.transform.position;
                if (Math.Abs(wallPosition.z % 1 - 0.5f) < 0.01f) {
                    var wallGridPosition = new GridPosition((int)wallPosition.x, (int)wallPosition.y, (int)(wallPosition.z + 0.5f));
                    var wallGridPosition2 = new GridPosition((int)wallPosition.x, (int)wallPosition.y, (int)(wallPosition.z - 0.5f));
                    Grid.AddWall(wallGridPosition, wallGridPosition2);
                }
                else {
                    var wallGridPosition = new GridPosition((int)(wallPosition.x - 0.5f), (int)wallPosition.y, (int)wallPosition.z);
                    var wallGridPosition2 = new GridPosition((int)(wallPosition.x + 0.5f), (int)wallPosition.y, (int)wallPosition.z);
                    Grid.AddWall(wallGridPosition, wallGridPosition2);
                }
            }
        }
    }
}