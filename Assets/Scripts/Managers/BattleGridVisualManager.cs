using System.Collections.Generic;
using Gangs.Abilities;
using Gangs.Battle.UI;
using Gangs.Core;
using Gangs.Grid;
using Gangs.UI;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleGridVisualManager : MonoBehaviour {
        public static BattleGridVisualManager Instance { get; private set; }
        
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

        private List<TargetTiles> _targetTiles = new();
        
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
            ClearSelectionCursor();
        }

        private void DrawTileDetails(Tile tile) {
            ClearTileDetails();
            
            if (tile == null) return;
            foreach (var wall in tile.Walls) {
                var coverType = BattleManager.Instance.GetCoverType(tile.GridPosition, wall.Key);
                if (coverType == CoverType.None) continue;
                var indicatorPos = GetIndicatorPosition(tile, wall.Key);
                var yPos = tile.GridPosition.Y + 0.5f;
                indicatorPos = Vector3.MoveTowards(indicatorPos, tile.GridPosition.ToVector3(), 0.15f);
                indicatorPos.y = yPos;
                var indicator = Instantiate(coverType == CoverType.Full ? fullCoverIndicatorPrefab : halfCoverIndicatorPrefab, indicatorPos, Quaternion.identity);
                if (wall.Key is CardinalDirection.East or CardinalDirection.West) {
                    indicator.transform.Rotate(Vector3.up, 90);
                }
                indicator.GetComponentInChildren<Renderer>().material.color = new Color(128 / 255f, 210 / 255f, 196 / 255f, 1f);
                _coverIndicators.Add(indicator);
            }
            
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

        private Vector3 GetIndicatorPosition(Tile tile, CardinalDirection direction) {
            var indicatorPos = tile.GridPosition.ToVector3();
            switch (direction) {
                case CardinalDirection.North:
                    indicatorPos.z += 0.5f;
                    break;
                case CardinalDirection.East:
                    indicatorPos.x += 0.5f;
                    break;
                case CardinalDirection.South:
                    indicatorPos.z -= 0.5f;
                    break;
                case CardinalDirection.West:
                    indicatorPos.x -= 0.5f;
                    break;
            }
            return indicatorPos;
        }

        private void ClearSelectionCursor() {
            if (_selectionCursor is null) return;
            Destroy(_selectionCursor);
            _selectionCursor = null;
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
        
        public void UpdateSelectionCursor(GameObject hoverTile) {
            if (_selectionCursor is null) {
                _selectionCursor = Instantiate(selectionCursorPrefab);
            }
            
            if (hoverTile is null) {
                _selectionCursor.SetActive(false);
                return;
            }
            
            _selectionCursor.SetActive(true);
            var gridPosition = new GridPosition(hoverTile.transform.position);
            
            _selectionCursor.transform.position = new Vector3 {
                x = gridPosition.X,
                y = gridPosition.Y + 0.01f,
                z = gridPosition.Z
            };

            if (!BattleInputManager.Instance.InputEnabled) return;
            if (BattleManager.Instance.CurrentAbility() == null) return;
            if (BattleManager.Instance.CurrentAbility().TargetingType == TargetingType.StandardMove) {
                DrawMovementPath(gridPosition);
                DrawTileDetails(BattleGridManager.Instance.GetTile(gridPosition));
            }
            if (BattleManager.Instance.CurrentAbility().TargetingType == TargetingType.EnemiesInLineOfSight) {
                _targetTiles.ForEach(t => t.Tiles.ForEach(tile => ColorTile(tile, Color.red)));
                var tile = BattleGridManager.Instance.GetTile(gridPosition);
                if (_targetTiles.Exists(t => t.Tiles.Contains(tile))) {
                    ColorTile(tile, Color.yellow);
                }
            }
        }
        
        public GameObject DrawWaypointIndicator(Tile tiles) {
            var pos = tiles.GridPosition;
            var waypoint = Instantiate(waypointIndicatorPrefab, new Vector3(pos.X, pos.Y + 0.02f, pos.Z), Quaternion.identity);
            waypoint.transform.Rotate(Vector3.right, 90);
            return waypoint;
        }

        private void DrawMoveRanges(List<TargetTiles> moveRanges) {
            _moveRangeLine.DestroyAllLines();
            _moveRangeLine.DrawMovementRangeBoundary(moveRanges);
        }

        private void ClearMoveRanges() => _moveRangeLine.DestroyAllLines();
        private void ClearWaypoints() => _movePathLine.ClearAllLines();
        private void ClearMovementPath() => _movePathLine.ClearMovementPath();

        private void DrawMovementPath(GridPosition endPosition) {
            ClearWaypoints();
            var endTile = BattleGridManager.Instance.GetTile(endPosition);
            if (!BattleManager.Instance.CurrentAbility().TargetTiles.Exists(t => t.Tiles.Contains(endTile))) return;
            var startTile = BattleManager.Instance.GetSelectedUnit().GridUnit.GetTile();
            var path = Pathfinder.FindOptimizedPath(startTile, endTile);
            _movePathLine.DrawMovementPath(path.DirectPathTiles);
        }

        public void ConvertMovementPathToWayPoint() => _movePathLine.ConvertMovementPathToWayPoint();

        public void ColorTile(Tile tile, Color color) {
            var tileGameObject = BattleManager.Instance.GetTileGameObject(tile.GridPosition);
            tileGameObject.GetComponentInChildren<Renderer>().material.color = color;
        }
        
        public void ClearAllTileColors() {
            foreach (var go in GameObject.FindGameObjectsWithTag("Tile")) {
                go.GetComponentInChildren<Renderer>().material.color = Color.white;
            }
        }

        public void NumberTile(Tile tile, float expectedDamageDifferential) {
            var tileGameObject = BattleManager.Instance.GetTileGameObject(tile.GridPosition);
            var text = Instantiate(new GameObject(), tileGameObject.transform);
            text.transform.position = tileGameObject.transform.position;
            text.transform.rotation = Quaternion.identity;
            text.transform.Rotate(Vector3.right, 90);
            text.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            var textMesh = text.AddComponent<TextMesh>();
            textMesh.text = expectedDamageDifferential.ToString("F1");
            _tileNumbers.Add(text);
        }

        public void DeleteAllTileNumbers() {
            foreach (var number in _tileNumbers) {
                Destroy(number);
            }
            _tileNumbers.Clear();
        }
        
        public void DrawTargetingTiles(Ability ability) {
            ResetAllVisuals();
            if (ability.TargetingType == TargetingType.StandardMove) {
                DrawMoveRanges(ability.TargetTiles);
            }
            
            if (ability.TargetingType == TargetingType.EnemiesInLineOfSight) {
                _targetTiles = ability.TargetTiles;
                _targetTiles.ForEach(t => t.Tiles.ForEach(tile => ColorTile(tile, Color.red)));
            }
        }
    }
}