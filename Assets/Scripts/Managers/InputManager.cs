using System;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }

        private Camera _camera;
        private Grid.Grid Grid => GridManager.Instance.Grid;

        public Tile HoverTile { get; private set; }

        public event Action<Tile> OnTileHovered;
        public event Action OnRightClick;
        public event Action<Tile> OnLeftClickTile; 

        private void Awake() {
            if (Instance != null && Instance != this) { 
                Destroy(this); 
            } 
            else { 
                Instance = this; 
            }
            
            _camera = Camera.main;
        }

        private void Update() {
            CheckHoverTile();
            CheckKeys();
        }

        private void CheckKeys() {
            if (Input.GetKeyDown(KeyCode.Q)) {
                _camera.transform.RotateAround(Vector3.zero, Vector3.up, 90);
            }
            
            if (Input.GetKeyDown(KeyCode.E)) {
                _camera.transform.RotateAround(Vector3.zero, Vector3.up, -90);
            }
            
            if (Input.GetKey(KeyCode.W)) {
                _camera.transform.Translate(Vector3.up * (Time.deltaTime * 10));
            }
            
            if (Input.GetKey(KeyCode.S)) {
                _camera.transform.Translate(Vector3.down * (Time.deltaTime * 10));
            }
            
            if (Input.GetKey(KeyCode.A)) {
                _camera.transform.Translate(Vector3.left * (Time.deltaTime * 10));
            }
            
            if (Input.GetKey(KeyCode.D)) {
                _camera.transform.Translate(Vector3.right * (Time.deltaTime * 10));
            }
            
            if (Input.GetKeyDown(KeyCode.Tab)) {
                GameManager.Instance.NextUnit();
            }

            if (Input.GetMouseButtonUp(0)) {
                LeftClick();
            }
            
            if (Input.GetMouseButtonUp(1)) {
                RightClick();
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && _camera.orthographicSize > 3) {
                _camera.orthographicSize -= 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && _camera.orthographicSize < 8) {
                _camera.orthographicSize += 1;
            }
            
            if (Input.GetMouseButton(2)) {
                _camera.transform.Translate(-Input.GetAxis("Mouse X") * (Time.deltaTime * 10), -Input.GetAxis("Mouse Y") * (Time.deltaTime * 10), 0);
            }

            // if (Input.GetKeyDown(KeyCode.M)) {
            //     // get all unit positions as debug
            //     foreach (var squad in GameManager.Instance.Squads) {
            //         foreach (var unit in squad.Units) {
            //             var tile = GameManager.Instance.GetSoldierTile(unit);
            //             var gridTile = Grid.FindGridUnit(unit.GridUnit);
            //             var pos = unit.UnitGameObject.Position;
            //             Debug.Log($"Unit {unit.Fighter.Name} at {pos} and tile {tile.GridPosition} and gridTile {gridTile.GridPosition}");
            //         }
            //     }
            // }
        }
        
        private void LeftClick() {
            var tile = GetMouseTile();
            if (tile == null) return;
            OnLeftClickTile?.Invoke(tile);
            
            if (tile.GridUnit != null) {
                ClickOnUnit();
            }
        }
        
        private void RightClick() {
            OnRightClick?.Invoke();
        }
        
        private void ClickOnUnit() {
            var unit = GameManager.Instance.FindUnit(HoverTile.GridUnit);
            if (unit.TurnTaken) return;
            if (GameManager.Instance.ActivatedUnit) return;
            SelectUnit(unit);
        }

        public void SelectUnit(Unit unit) {
            if (unit == GameManager.Instance.SelectedUnit) return;
            GameManager.Instance.SelectedUnit.SelectedAbility?.Deselect();
        }
        
        private void CheckHoverTile() {
            var tile = GetMouseTile();
            if (tile == HoverTile) return;
            HoverTile = tile;
            OnTileHovered?.Invoke(HoverTile);
            GridVisualManager.Instance.UpdateSelectionCursor(HoverTile);
        }

        private Tile GetMouseTile() {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return null;
            var tileGameObject = hit.collider.gameObject.transform.parent.transform;
            if (!tileGameObject.CompareTag($"Tile")) return null;
            return Grid.GetTile(tileGameObject.position);
        }
    }
}
