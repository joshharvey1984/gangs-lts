using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities.Structs;
using Gangs.Grid;
using Gangs.Managers;
using Gangs.UI;
using UnityEngine;

namespace Gangs.Battle.GameObjects {
    public class UnitGameObject : MonoBehaviour {
        public BattleUnit BattleUnit { get; set; }
        public GameObject selectionCircleObject;
        public GameObject modelObject;
        private SelectionCircle SelectionCircle => selectionCircleObject.GetComponent<SelectionCircle>();

        private Vector3 Position {
            set => gameObject.transform.position = value;
            get => gameObject.transform.position;
        }
        
        private List<MoveWaypoint> _moveWaypoints;
        private float MoveSpeed => 5.5f;
        
        public event Action<GridPosition> UnitNewPosition;
        public event Action OnMoveComplete;
        
        private void Awake() {
            modelObject = gameObject.transform.GetChild(0).gameObject;
        }

        private void Update() {
            if (_moveWaypoints is {Count: > 0}) { HandleMovement(); }
        }
        
        public void Move(List<MoveWaypoint> tiles) {
            _moveWaypoints = tiles;
        }
        
        public void Eliminate() {
            Destroy(gameObject);
        }

        public void SetBattleUnit(BattleUnit unit) {
            BattleUnit = unit;
            BattleUnit.OnSelected += SetSelected;
            BattleUnit.OnDeselected += SetDeselected;
            BattleUnit.OnMoveUnitTile += MoveUnit;
        }
        
        private void MoveUnit(BattleUnit unit, MoveWaypoint waypoint) {
            _moveWaypoints = new List<MoveWaypoint> { waypoint };
        }

        private void SetSelected() => SelectionCircle.SetState(SelectionCircle.State.Selected);
        private void SetDeselected() => SelectionCircle.SetState(SelectionCircle.State.Available);
        
        private void HandleMovement() {
            var nextTile = _moveWaypoints.First().DirectPathTiles.First();
            var tilePosition = new Vector3(nextTile.GridPosition.X, nextTile.GridPosition.Y, nextTile.GridPosition.Z);
            Position = Vector3.MoveTowards(Position, tilePosition, MoveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(gameObject.transform.position, tilePosition) < 0.01f) {
                Position = tilePosition;
                _moveWaypoints.First().DirectPathTiles.RemoveAt(0);
                if (_moveWaypoints.First().DirectPathTiles.Count == 0) {
                    _moveWaypoints.RemoveAt(0);
                }
                if (_moveWaypoints.Count == 0) {
                    BattleManager.Instance.MoveUnit(BattleUnit, new GridPosition(Position));
                    BattleUnit.MoveNextWaypointTile();
                    return;
                }
            }
            
            BattleManager.Instance.MoveUnit(BattleUnit, new GridPosition(Position));
        }
    }
}