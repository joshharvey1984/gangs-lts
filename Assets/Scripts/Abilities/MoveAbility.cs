using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities.Structs;
using Gangs.Data;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Abilities {
    public class MoveAbility : Ability {
        private List<MoveRange> _moveRanges;
        private List<MoveWaypoint> _moveWaypoints;
        
        public MoveAbility(Unit unit) : base(unit) {
            ButtonText = "Move";
            TargetingType = TargetingType.StandardMove;
        }
        
        private Tile SoldierTile => GameManager.Instance.GetSoldierTile(Unit);
        private int MovePoints => Unit.GetAttribute(FighterAttribute.Movement) * 10;
        private int MovePointsUsed => _moveWaypoints.Sum(w => w.Cost);
        private int MovePointsRemaining => MovePoints * Unit.ActionPointsRemaining - MovePointsUsed;
        private bool MaxMovementReached => MovePointsUsed > MovePoints * Unit.ActionPointsRemaining - 10;
        
        public override void Select() {
            base.Select();
            InputManager.Instance.OnTileHovered += TileHovered;
            InputManager.Instance.OnRightClick += ResetMove;
            InputManager.Instance.OnLeftClickTile += LeftClickTile;
            SetInitialWaypoint();
            GridVisualManager.Instance.DrawTileDetails(InputManager.Instance.HoverTile);
        }
        
        public override void Deselect() {
            InputManager.Instance.OnRightClick -= ResetMove;
            InputManager.Instance.OnTileHovered -= TileHovered;
            InputManager.Instance.OnLeftClickTile -= LeftClickTile;
            CancelMove();
            base.Deselect();
        }

        protected override void Execute() {
            base.Execute();
            GridVisualManager.Instance.ClearWaypoints();
            GridVisualManager.Instance.ClearMoveRanges();
            var apSpent = (int)Math.Ceiling((double)MovePointsUsed / MovePoints);
            Unit.SpendActionPoints(apSpent);
            Unit.UnitGameObject.OnMoveComplete += MoveComplete;
            Unit.UnitGameObject.Move(new List<MoveWaypoint>(_moveWaypoints));
            ResetMoveWaypoints();
        }

        private void TileHovered(Tile tile) {
            if (MaxMovementReached) return;
            DrawMovePath(tile);
            GridVisualManager.Instance.DrawTileDetails(tile);
        }

        private void LeftClickTile(Tile tile) {
            if (!_moveRanges.SelectMany(t => t.Tiles).ToList().Contains(tile)) return;
            if (tile.GridUnit != null) return;
            if (_moveWaypoints.Last().DirectPathTiles.Last() == tile) Execute();
            else AddWaypoint(tile);
        }

        private void AddWaypoint(Tile tile) {
            var lastWaypoints = _moveWaypoints.Select(w => w.DirectPathTiles.Last()).ToList();
            if (lastWaypoints.Contains(tile)) {
                ResetMove();
                return;
            }
                
            var path = Pathfinder.FindOptimizedPath(_moveWaypoints.Last().DirectPathTiles.Last(), tile);
            _moveWaypoints.Add(new MoveWaypoint {
                DirectPathTiles = path.DirectPathTiles,
                Tiles = path.PathTiles,
                Cost = path.Cost,
                Indicator = GridVisualManager.Instance.DrawWaypointIndicator(tile)
            });
            
            GridVisualManager.Instance.ConvertMovementPathToWayPoint();
            CalculateMoveRange();
        }
        
        private void SetInitialWaypoint() {
            _moveWaypoints ??= new List<MoveWaypoint>();
            if (_moveWaypoints.Count > 0) ResetMoveWaypoints();
            _moveWaypoints.Add(new MoveWaypoint {
                DirectPathTiles = new List<Tile> { SoldierTile },
                Cost = 0
            });
            
            CalculateMoveRange();
        }
        
        private void CalculateMoveRange() {
            var totalMoveRange = GetMoveRange();
            var moveRanges = new List<MoveRange>();
            
            for (var i = 0; i < Unit.ActionPointsRemaining; i++) {
                var moveRange = Unit.GetAttribute(FighterAttribute.Movement) * 10 * (i + 1) - MovePointsUsed;
                var m = totalMoveRange.Where(kvp => kvp.Value <= moveRange);
                var keys = m.Select(kvp => kvp.Key).ToList();
                var moveRangeList = new MoveRange(i, keys);
                moveRanges.Add(moveRangeList);
            }
            
            GridVisualManager.Instance.DrawMoveRanges(moveRanges.OrderBy(m => m.ActionPoint).ToList());
            _moveRanges = moveRanges;
        }

        private Dictionary<Tile, int> GetMoveRange() {
            var originTile = _moveWaypoints.Last().DirectPathTiles.Last();
            var remainingMove = MovePointsRemaining;
            return Pathfinder.CalculateMoveRange(originTile, remainingMove);
        }
        
        private void ResetMoveWaypoints() {
            foreach (var moveWaypoint in _moveWaypoints) {
                moveWaypoint.DestroyIndicator();
            }
            _moveWaypoints.Clear();
        }
        
        private void CancelMove() {
            ResetMoveWaypoints();
            GridVisualManager.Instance.ClearWaypoints();
            GridVisualManager.Instance.ClearMoveRanges();
        }
        
        private void ResetMove() {
            CancelMove();
            SetInitialWaypoint();
            DrawMovePath(InputManager.Instance.HoverTile);
            GridVisualManager.Instance.DrawTileDetails(InputManager.Instance.HoverTile);
        }

        private void DrawMovePath(Tile tile) {
            GridVisualManager.Instance.ClearMovementPath();
            if (tile == null) return;
            if (!_moveRanges.SelectMany(t => t.Tiles).ToList().Contains(tile)) return;

            foreach (var moveWaypoint in _moveWaypoints) {
                GridVisualManager.Instance.DrawMovementPath(moveWaypoint.DirectPathTiles);
            }

            var pathTiles = Pathfinder.FindOptimizedPath(_moveWaypoints.Last().DirectPathTiles.Last(), tile);
            GridVisualManager.Instance.DrawMovementPath(pathTiles.DirectPathTiles);
        }

        private void MoveComplete() {
            ResetMove();
            Unit.UnitGameObject.OnMoveComplete -= MoveComplete;
            Finish();
        }
    }
}