using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities.Structs;
using Gangs.Grid;
using Gangs.UI;
using UnityEngine;

namespace Gangs.GameObjects {
    public class UnitGameObject : MonoBehaviour {
        public GameObject selectionCircleObject;
        private SelectionCircle SelectionCircle => selectionCircleObject.GetComponent<SelectionCircle>();
        public Vector3 Position {
            private set => gameObject.transform.position = value;
            get => gameObject.transform.position;
        }
        
        private List<MoveWaypoint> _moveWaypoints;
        private const float MoveSpeed = 5.5f;
        
        public event Action<GridPosition> UnitNewPosition;
        public event Action OnMoveComplete;

        private void Update() {
            if (_moveWaypoints is {Count: > 0}) { HandleMovement(); }
        }
        
        public void Move(List<MoveWaypoint> tiles) {
            _moveWaypoints = tiles;
        }

        public void SetSelected(SelectionCircle.State state) => SelectionCircle.SetState(state);
        
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
                    UnitNewPosition?.Invoke(new GridPosition(Position));
                    OnMoveComplete?.Invoke();
                    return;
                }
            }
            
            UnitNewPosition?.Invoke(new GridPosition(Position));
        }
    }
}