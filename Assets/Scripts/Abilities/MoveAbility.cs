using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities.Structs;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Core;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Abilities {
    public class MoveAbility : Ability {
        private List<MoveRange> _moveRanges;
        private List<MoveWaypoint> _moveWaypoints;
        
        public MoveAbility(BattleUnit battleUnit, BattleGrid battleGrid) : base(battleUnit, battleGrid) {
            ButtonText = "Move";
            TargetingType = TargetingType.StandardMove;
        }

        private Tile SoldierTile => BattleUnit.GridUnit.GetTile();
        private int MovePoints => BattleUnit.GetAttributeValue(UnitAttributeType.Movement) * 10;
        private int MovePointsUsed => _moveWaypoints.Sum(w => w.Cost);
        private int MovePointsRemaining => MovePoints * BattleUnit.ActionPointsRemaining - MovePointsUsed;
        private bool MaxMovementReached => MovePointsUsed > MovePoints * BattleUnit.ActionPointsRemaining - 10;
        
        public override void Select() {
            base.Select();
            if (BattleUnit.IsPlayerControlled) {
                InputManager.Instance.OnTileHovered += TileHovered;
                InputManager.Instance.OnRightClick += ResetMove;
                InputManager.Instance.OnLeftClickTile += LeftClickTile;
            }

            CalculateMoveRange();
            //GridVisualManager.Instance.DrawTileDetails(InputManager.Instance.HoverTile);
        }
        
        public override void Deselect() {
            if (BattleUnit.IsPlayerControlled) {
                InputManager.Instance.OnRightClick -= ResetMove;
                InputManager.Instance.OnTileHovered -= TileHovered;
                InputManager.Instance.OnLeftClickTile -= LeftClickTile;
            }

            CancelMove();
            base.Deselect();
        }

        public override void Execute() {
            base.Execute();
            // if (BattleUnit.IsPlayerControlled) {
            //     GridVisualManager.Instance.ClearWaypoints();
            //     GridVisualManager.Instance.ClearMoveRanges();
            // }
            
            var apSpent = (int)Math.Ceiling((double)MovePointsUsed / MovePoints);
            BattleUnit.SpendActionPoints(apSpent);
            _moveWaypoints.Last().Tiles.ForEach(MoveToTile);
            //BattleUnit.UnitGameObject.OnMoveComplete += MoveComplete;
            //BattleUnit.UnitGameObject.Move(new List<MoveWaypoint>(_moveWaypoints));
            Debug.Log($"{BattleUnit.Unit.Name} moved to {BattleUnit.GridUnit.GetTile()}");
            ResetMoveWaypoints();
            
            MoveComplete();
        }
        
        private void MoveToTile(Tile tile) {
            BattleGrid.MoveUnit(BattleUnit, tile);
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

        public void AddWaypoint(Tile tile) {
            var lastWaypoints = _moveWaypoints.Select(w => w.DirectPathTiles.Last()).ToList();
            if (lastWaypoints.Contains(tile)) {
                ResetMove();
                return;
            }
                
            var path = Pathfinder.FindOptimizedPath(_moveWaypoints.Last().DirectPathTiles.Last(), tile);
            _moveWaypoints.Add(new MoveWaypoint {
                DirectPathTiles = path.DirectPathTiles,
                Tiles = path.PathTiles,
                Cost = path.Cost
            });
            
            //GridVisualManager.Instance.ConvertMovementPathToWayPoint();
            _moveRanges = CalculateMoveRange();
        }
        
        public List<MoveRange> CalculateMoveRange() {
            if (_moveWaypoints == null || _moveWaypoints.Count == 0) {
                _moveWaypoints = new List<MoveWaypoint> {
                    new() {
                        DirectPathTiles = new List<Tile> { SoldierTile },
                        Cost = 0
                    }
                };
            }
            
            var totalMoveRange = GetMoveRange();
            var moveRanges = new List<MoveRange>();
            
            for (var i = 0; i < BattleUnit.ActionPointsRemaining; i++) {
                var moveRange = BattleUnit.GetAttributeValue(UnitAttributeType.Movement) * 10 * (i + 1) - MovePointsUsed;
                var m = totalMoveRange.Where(kvp => kvp.Value <= moveRange);
                var keys = m.Select(kvp => kvp.Key).ToList();
                var moveRangeList = new MoveRange(i, keys);
                moveRanges.Add(moveRangeList);
            }

            if (BattleUnit.IsPlayerControlled) {
                GridVisualManager.Instance.DrawMoveRanges(moveRanges.OrderBy(m => m.ActionPoint).ToList());
            }
            
            _moveRanges = moveRanges;

            return moveRanges;
        }

        private Dictionary<Tile, int> GetMoveRange() {
            var originTile = _moveWaypoints.Last().DirectPathTiles.Last();
            var remainingMove = MovePointsRemaining;
            return Pathfinder.CalculateMoveRange(originTile, remainingMove);
        }
        
        private void ResetMoveWaypoints() {
            _moveWaypoints.Clear();
        }
        
        private void CancelMove() {
            ResetMoveWaypoints();
            
            if (BattleUnit.IsPlayerControlled) {
                GridVisualManager.Instance.ClearWaypoints();
                GridVisualManager.Instance.ClearMoveRanges();
            }
        }
        
        private void ResetMove() {
            CancelMove();
            CalculateMoveRange();
            DrawMovePath(InputManager.Instance.HoverTile);

            if (BattleUnit.IsPlayerControlled) {
                GridVisualManager.Instance.DrawTileDetails(InputManager.Instance.HoverTile);
            }
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
            //ResetMove();
            //BattleUnit.UnitGameObject.OnMoveComplete -= MoveComplete;
            Finish();
        }
    }
}