using System;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleInputManager : MonoBehaviour {
        public static BattleInputManager Instance { get; private set; }

        private Camera _camera;

        public GameObject HoverTile { get; private set; }
        
        public bool InputEnabled { get; private set; } = false;

        public event Action<GameObject> OnTileHovered;
        public event Action OnRightClick;
        public event Action<Tile> OnLeftClickTile; 

        private void Awake() {
            if (Instance is not null && Instance != this) Destroy(this); 
            else Instance = this;
            
            _camera = Camera.main;
        }

        private void Update() {
            if (!InputEnabled) return;
            CheckHoverTile();
            CheckKeys();
        }
        
        public void SetPlayerControl(bool playerControl = true) {
            InputEnabled = playerControl;
        }

        private void CheckKeys() {
            if (Input.GetKeyUp(KeyCode.Q)) {
                _camera.transform.RotateAround(Vector3.zero, Vector3.up, 90);
            }
            
            if (Input.GetKeyUp(KeyCode.E)) {
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
            
            if (Input.GetKeyUp(KeyCode.Tab)) {
                BattleManager.Instance.NextUnit();
            }
            
            if (Input.GetKeyUp(KeyCode.Space)) {
                BattleManager.Instance.EndUnitTurn();
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
        }
        
        private void LeftClick() {
            var tile = GetMouseTileGameObject();
            if (tile is null) return;
            var gridPosition = new GridPosition(tile.transform.position);
            var gridTile = BattleGridManager.Instance.GetTile(gridPosition);
            OnLeftClickTile?.Invoke(gridTile);
            
            // if (tile.GridUnit != null) {
            //     ClickOnUnit();
            // }
        }
        
        private void RightClick() {
            OnRightClick?.Invoke();
        }
        
        // private void ClickOnUnit() {
        //     var unit = BattleManager.Instance.FindUnit(HoverTile.GridUnit);
        //     if (unit == null) return;
        //     if (!unit.IsPlayerControlled) return;
        //     if (unit.TurnTaken) return;
        //     if (BattleManager.Instance.ActivatedUnit) return;
        //     SelectUnit(unit);
        // }
        //
        // public void SelectUnit(BattleUnit battleUnit) {
        //     if (battleUnit == BattleManager.Instance.SelectedBattleUnit) return;
        //     BattleManager.Instance.SelectedBattleUnit.SelectedAbility?.Deselect();
        // }
        
        private void CheckHoverTile() {
            var tile = GetMouseTileGameObject();
            if (tile == HoverTile) return;
            HoverTile = tile;
            OnTileHovered?.Invoke(HoverTile);
            BattleGridVisualManager.Instance.UpdateSelectionCursor(HoverTile);
        } 

        private GameObject GetMouseTileGameObject() {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return null;
            var tileGameObject = hit.collider.gameObject.transform.parent.transform;
            return !tileGameObject.CompareTag($"Tile") ? null : tileGameObject.gameObject;
        }
    }
}
