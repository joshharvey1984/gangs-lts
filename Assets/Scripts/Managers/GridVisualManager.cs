using System.Collections.Generic;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
using Gangs.Battle.UI;
using Gangs.Grid;
using Gangs.UI;
using UnityEngine;

namespace Gangs.Managers {
    public class GridVisualManager : MonoBehaviour {
        public static GridVisualManager Instance { get; private set; }
        
        [SerializeField]
        private GameObject moveRangeLineRendererObject;
        private MoveRangeLine _moveRangeLine;
        
        [SerializeField]
        private GameObject movePathLineRendererObject;
        private MovePathLine _movePathLine;
        
        [SerializeField]
        private GameObject waypointIndicatorPrefab;
        
        [SerializeField]
        private GameObject selectionCursorPrefab;
        private GameObject _selectionCursor;
        
        [SerializeField]
        private GameObject fullCoverIndicatorPrefab;
        [SerializeField]
        private GameObject halfCoverIndicatorPrefab;
        private readonly List<GameObject> _coverIndicators = new();
        
        [SerializeField]
        private GameObject debugLosIndicatorPrefab;
        private readonly List<GameObject> _debugLosIndicators = new();
        
        private List<GameObject> _tileNumbers = new();
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            _moveRangeLine = moveRangeLineRendererObject.GetComponent<MoveRangeLine>();
            _movePathLine = movePathLineRendererObject.GetComponent<MovePathLine>();
        }
        
        public void ResetAllVisuals() {
            ClearMoveRanges();
            ClearWaypoints();
            ClearMovementPath();
            ClearTileDetails();
            ClearAllTileColors();
        }
        
        public void DrawTileDetails(Tile tile) {
            // ClearTileDetails();
            //
            // if (tile == null) return;
            // foreach (var wall in tile.Walls) {
            //     var wallGameObject = BattleManager.Instance.GetWallGameObject(wall.Value);
            //     if (wallGameObject == null) continue;
            //     var wallScript = wallGameObject.GetComponent<WallGameObject>();
            //     if (wallScript.CoverType == CoverType.None) continue;
            //     var indicatorPos = wallGameObject.transform.position;
            //     var yPos = tile.GridPosition.Y + 0.5f;
            //     indicatorPos = Vector3.MoveTowards(indicatorPos, tile.GridPosition.ToVector3(), 0.15f);
            //     indicatorPos.y = yPos;
            //     var indicator = Instantiate(wallScript.CoverType == CoverType.Full ? fullCoverIndicatorPrefab : halfCoverIndicatorPrefab, indicatorPos, Quaternion.identity);
            //     if (wall.Key is CardinalDirection.East or CardinalDirection.West) {
            //         indicator.transform.Rotate(Vector3.up, 90);
            //     }
            //     indicator.GetComponentInChildren<Renderer>().material.color = new Color(128 / 255f, 210 / 255f, 196 / 255f, 1f);
            //     _coverIndicators.Add(indicator);
            // }
            //
            // if (DebugManager.Instance.DebugMode) {
            //     // draw line of sight
            //     var lineOfSight = tile.LineOfSightGridPositions;
            //     foreach (var pos in lineOfSight) {
            //         var tileGameObject = BattleManager.Instance.GetTileGameObject(pos);
            //         if (tileGameObject == null) continue;
            //         var indicator = Instantiate(debugLosIndicatorPrefab, tileGameObject.transform.position, Quaternion.identity);
            //         _debugLosIndicators.Add(indicator);
            //     }
            // }
        }

        private void ClearTileDetails() {
            foreach (var indicator in _coverIndicators) {
                Destroy(indicator);
            }
            _coverIndicators.Clear();
            
            foreach (var indicator in _debugLosIndicators) {
                Destroy(indicator);
            }
            _debugLosIndicators.Clear();
        }
        
        public void UpdateSelectionCursor(Tile hoverTile) {
            if (_selectionCursor == null) {
                _selectionCursor = Instantiate(selectionCursorPrefab);
            }
            
            if (hoverTile == null) {
                _selectionCursor.SetActive(false);
                return;
            }
            
            _selectionCursor.SetActive(true);
            
            _selectionCursor.transform.position = new Vector3 {
                x = hoverTile.GridPosition.X,
                y = hoverTile.GridPosition.Y + 0.01f,
                z = hoverTile.GridPosition.Z
            };
        }
        
        public GameObject DrawWaypointIndicator(Tile tiles) {
            var pos = tiles.GridPosition;
            var waypoint = Instantiate(waypointIndicatorPrefab, new Vector3(pos.X, pos.Y + 0.02f, pos.Z), Quaternion.identity);
            waypoint.transform.Rotate(Vector3.right, 90);
            return waypoint;
        }
        
        public void DrawMoveRanges(List<TargetTiles> moveRanges) {
            _moveRangeLine.DestroyAllLines();
            _moveRangeLine.DrawMovementRangeBoundary(moveRanges);
        }
        public void ClearMoveRanges() => _moveRangeLine.DestroyAllLines();
        public void ClearWaypoints() => _movePathLine.ClearAllLines();
        public void ClearMovementPath() => _movePathLine.ClearMovementPath();
        public void DrawMovementPath(List<Tile> tiles) => _movePathLine.DrawMovementPath(tiles);
        public void ConvertMovementPathToWayPoint() => _movePathLine.ConvertMovementPathToWayPoint();

        // public void ColorTile(Tile tile, Color color) {
        //     var tileGameObject = BattleManager.Instance.GetTileGameObject(tile.GridPosition);
        //     tileGameObject.GetComponentInChildren<Renderer>().material.color = color;
        // }
        
        public void ClearAllTileColors() {
            foreach (var go in GameObject.FindGameObjectsWithTag("Tile")) {
                go.GetComponentInChildren<Renderer>().material.color = Color.white;
            }
        }

        // public void NumberTile(Tile tile, float expectedDamageDifferential) {
        //     var tileGameObject = BattleManager.Instance.GetTileGameObject(tile.GridPosition);
        //     var text = Instantiate(new GameObject(), tileGameObject.transform);
        //     text.transform.position = tileGameObject.transform.position;
        //     text.transform.rotation = Quaternion.identity;
        //     text.transform.Rotate(Vector3.right, 90);
        //     text.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //     var textMesh = text.AddComponent<TextMesh>();
        //     textMesh.text = expectedDamageDifferential.ToString("F1");
        //     _tileNumbers.Add(text);
        // }

        public void DeleteAllTileNumbers() {
            foreach (var number in _tileNumbers) {
                Destroy(number);
            }
            _tileNumbers.Clear();
        }

        public void DrawTargetingTiles(List<TargetTiles> targetTiles, TargetingType targetingType) {
            ResetAllVisuals();
            if (targetingType == TargetingType.StandardMove) {
                DrawMoveRanges(targetTiles);
            }
        }
    }
}