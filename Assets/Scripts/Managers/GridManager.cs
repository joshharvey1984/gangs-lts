using System;
using Gangs.GameObjects;
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
        
        public void SetupGrid() {
            SetupTiles();
            SetupProps();
            SetupWalls();
            SetupLadders();
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
                var propScript = prop.GetComponent<PropGameObject>();
                var propPosition = prop.transform.position;
                propScript.Prop = Grid.AddProp(new GridPosition((int)propPosition.x, (int)propPosition.y, (int)propPosition.z), propScript.LineOfSightBlocker);
            }
        }

        private void SetupLadders() {
            var ladders = GameObject.FindGameObjectsWithTag($"Ladder");
            foreach (var ladder in ladders) {
                var ladderPosition = ladder.transform.position;
                
                var upperLadderPosition = ladder.transform.rotation.eulerAngles.y switch {
                    0 => new Vector3(ladderPosition.x, ladderPosition.y + 1, ladderPosition.z + 1),
                    180 => new Vector3(ladderPosition.x, ladderPosition.y + 1, ladderPosition.z - 1),
                    90 => new Vector3(ladderPosition.x + 1, ladderPosition.y + 1, ladderPosition.z),
                    270 => new Vector3(ladderPosition.x - 1, ladderPosition.y + 1, ladderPosition.z),
                    _ => new Vector3()
                };

                var ladderGridPosition = new GridPosition(ladderPosition);
                var upperLadderGridPosition = new GridPosition(upperLadderPosition);
                
                Grid.AddLadder(ladderGridPosition, upperLadderGridPosition);
            }
        }
        
        private void SetupWalls() {
            var wallGameObjects = GameObject.FindGameObjectsWithTag($"Wall");
            foreach (var wallGameObject  in wallGameObjects) {
                var wallScript = wallGameObject.GetComponent<WallGameObject>();
                var wallPosition = wallGameObject.transform.position;
                if (Math.Abs(wallPosition.z % 1 - 0.5f) < 0.01f) {
                    var wallGridPosition = new GridPosition((int)wallPosition.x, (int)wallPosition.y, (int)(wallPosition.z + 0.5f));
                    var wallGridPosition2 = new GridPosition((int)wallPosition.x, (int)wallPosition.y, (int)(wallPosition.z - 0.5f));
                    wallScript.Wall = Grid.AddWall(wallGridPosition, wallGridPosition2, wallScript.LineOfSightBlocker);
                }
                else {
                    var wallGridPosition = new GridPosition((int)(wallPosition.x - 0.5f), (int)wallPosition.y, (int)wallPosition.z);
                    var wallGridPosition2 = new GridPosition((int)(wallPosition.x + 0.5f), (int)wallPosition.y, (int)wallPosition.z);
                    wallScript.Wall = Grid.AddWall(wallGridPosition, wallGridPosition2, wallScript.LineOfSightBlocker);
                }
            }
        }

        public CoverType CheckTileCover(Tile fromTile, Tile targetTile) {
            var coverType = CoverType.None;
            foreach (var wall in targetTile.Walls) {
                if (GridPosition.CheckDirection(targetTile.GridPosition, fromTile.GridPosition, wall.Key)) {
                    var wallCoverType = GameManager.Instance.GetWallGameObject(wall.Value).GetComponent<WallGameObject>().CoverType;
                    if (wallCoverType > coverType) {
                        coverType = wallCoverType;
                    }
                }
            }
            
            foreach (var prop in targetTile.NeighbourProps) {
                if (GridPosition.CheckDirection(fromTile.GridPosition, targetTile.GridPosition, prop.Key)) {
                    var propCoverType = GameManager.Instance.GetPropGameObject(prop.Value).GetComponent<PropGameObject>().CoverType;
                    if (propCoverType > coverType) {
                        coverType = propCoverType;
                    }
                }
            }
            
            return coverType;
        }
    }
}