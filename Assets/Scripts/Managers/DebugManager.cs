using Gangs.Data;
using UnityEngine;

namespace Gangs.Managers {
    public class DebugManager : MonoBehaviour {
        public static DebugManager Instance { get; private set; }
        private GameManager GM => GameManager.Instance;
        
        public bool debugMode;
        
        public PathfindingMode pathfindingMode = PathfindingMode.AStarWithLerpAndSmoothing;
        public bool lineOfSite;
        
        public enum PathfindingMode {
            BasicAStar,
            AStarWithLerp,
            AStarWithLerpAndSmoothing
        }
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void ToggleDebugMode() {
            debugMode = !debugMode;
            if (!debugMode) {
                pathfindingMode = PathfindingMode.AStarWithLerpAndSmoothing;
            }
            
            // InputManager.Instance.DrawMoveRanges();
            // InputManager.Instance.DrawMovePath();
            //InputManager.Instance.GetLineOfSight();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Z)) {
                ToggleDebugMode();
            }

            if (debugMode) {
                if (Input.GetKeyUp(KeyCode.M)) {
                    TogglePathfindingMode();
                }

                if (Input.GetKeyUp(KeyCode.U)) {
                    MoveUnitToHoverTile();
                }
                
                if (Input.GetKeyUp(KeyCode.L)) {
                    ToggleLineOfSite();
                }
            }
        }

        private void ToggleLineOfSite() {
            lineOfSite = !lineOfSite;
            //InputManager.Instance.GetLineOfSight();
        }

        private void MoveUnitToHoverTile() {
            if (InputManager.Instance.HoverTile == null) {
                return;
            }

            var selectedSoldier = GM.SquadTurn.SelectedUnit.UnitGameObject;
            if (selectedSoldier == null) {
                return;
            }

            var unitTile = GM.GetSoldierTile(GM.SquadTurn.SelectedUnit);
            if (unitTile == null) {
                return;
            }

            GameObject unitGo = null;
            var units = GameObject.FindGameObjectsWithTag("Soldier");
            foreach (var unit in units) {
                if (unit.transform.position == new Vector3(unitTile.GridPosition.X, unitTile.GridPosition.Y, unitTile.GridPosition.Z)) {
                    unitGo = unit;
                }
            }
            
            if (unitGo == null) {
                Debug.LogError("Could not find unit game object.");
                return;
            }
            var tile = InputManager.Instance.HoverTile;
            var newTilePos = new Vector3(tile.GridPosition.X, tile.GridPosition.Y, tile.GridPosition.Z);
            unitGo.transform.position = newTilePos;
            
            var movingUnit = unitTile.GridUnit;
            unitTile.GridUnit = null;
            InputManager.Instance.HoverTile.GridUnit = movingUnit;
            
            // InputManager.Instance.DrawMoveRanges();
            // InputManager.Instance.DrawMovePath();
            //InputManager.Instance.GetLineOfSight();
        }

        private void TogglePathfindingMode() {
            pathfindingMode = pathfindingMode switch {
                PathfindingMode.AStarWithLerpAndSmoothing => PathfindingMode.BasicAStar,
                PathfindingMode.BasicAStar => PathfindingMode.AStarWithLerp,
                PathfindingMode.AStarWithLerp => PathfindingMode.AStarWithLerpAndSmoothing,
                _ => pathfindingMode
            };
            
            //InputManager.Instance.DrawMovePath();
        }
    }
}
