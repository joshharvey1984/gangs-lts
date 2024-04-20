using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.AI {
    public static class EnemyAI {
        public static void TakeTurn(Unit unit) {
            if (BattleManager.Instance.SquadTurn is not AISquad) {
                Debug.LogError("EnemyAI.TakeTurn() called on a non-AI squad turn!");
                return;
            }
            
            var moveAbility = unit.Abilities[0] as MoveAbility;
            var fireAbility = unit.Abilities[1] as FireAbility;
            
            var currentTile = GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            
            var lastSeenEnemies = BattleManager.Instance.SquadTurn.EnemyLastSeen;
            var enemiesInSight = GetEnemiesInSight();
            
            var enemyData = new List<TargetTiles>();
            
            foreach (var (enemy, tile) in lastSeenEnemies) {
                enemyData.Add(new TargetTiles {
                    Unit = enemy,
                    Tile = tile,
                    Type = enemiesInSight.Contains(enemy) 
                        ? TargetTiles.TargetTileType.VisibleEnemy 
                        : TargetTiles.TargetTileType.LastSeenEnemy
                });
            }
            
            if (enemyData.Count == 0) {
                var centreTile = GridManager.Instance.Grid.GetRandomCenterTile();
                enemyData.Add(new TargetTiles {
                    Unit = null,
                    Tile = centreTile,
                    Type = TargetTiles.TargetTileType.Search
                });
            }
            
            var squad = BattleManager.Instance.SquadTurn as AISquad;
            var weightings = squad!.Weightings;
            
            var moveRange = moveAbility!.CalculateMoveRange();
            var bestMoveTiles = FindBestTile(unit, moveRange, enemyData);
            var bestMove = bestMoveTiles.Take(3).ElementAt(Random.Range(0, 3));
            
            var bestMoveTileValue = bestMove.Value;
            var currentTileValue = GetTilePointValue(currentTile, enemyData, weightings, unit.ActionPointsRemaining);
            
            if (bestMoveTileValue <= currentTileValue) {
                if (unit.GetEnemiesInLineOfSight().Count > 0) {
                    var target = unit.GetEnemiesInLineOfSight().First();
                    var targetTile = GridManager.Instance.Grid.FindGridUnit(target.GridUnit);
                    fireAbility!.Select();
                    fireAbility!.LeftClickTile(targetTile);
                }
                else {
                    BattleManager.Instance.SquadTurn.EndUnitTurn();
                }
                
                return;
            }
            
            moveAbility!.AddWaypoint(bestMove.Key);
            moveAbility!.Execute();
        }
        
        private static Dictionary<Tile, float> FindBestTile(Unit unit, List<MoveRange> moveRange, List<TargetTiles> targetTiles) {
            var squad = BattleManager.Instance.SquadTurn as AISquad;
            var weightings = squad!.Weightings;
            
            var candidateMoves = new Dictionary<Tile, float>();
            foreach (var move in moveRange) {
                foreach (var tile in move.Tiles) {
                    if (tile.GridUnit != null) continue;
                    var pointValue = GetTilePointValue(tile, targetTiles, weightings, move.ActionPoint);
                    if (!candidateMoves.TryAdd(tile, pointValue)) candidateMoves[tile] += pointValue;
                }
            }
            
            return candidateMoves.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static float GetTilePointValue(Tile tile, List<TargetTiles> targetTiles, EnemyAIWeightings weightings, int apRemaining) {
            var pointValue = 0f;
            var cover = GetTileCoverFromEnemyTiles(tile, targetTiles);
            if (cover is {Count: > 0}){
                var worstCover = cover.Values.Min();
                if (worstCover == CoverType.Half) pointValue += weightings.HalfCoverWeight;
                if (worstCover == CoverType.Full) pointValue += weightings.FullCoverWeight;
            }
            
            if (HeightAdvantage(tile, targetTiles)) pointValue += weightings.HeightAdvantageWeight;
            if (CanFlank(tile, targetTiles)) pointValue += weightings.CanFlankWeight;
            if (IsFlanked(tile, targetTiles)) pointValue += weightings.IsFlankedWeight;
            pointValue += DistanceCheck(tile, targetTiles) * -weightings.DistanceCheckWeight;
            if (apRemaining > 0) pointValue += weightings.RemainingActionPointWeight;
            
            return pointValue;
        }
        
        private static int DistanceCheck(Tile tile, List<TargetTiles> targetTiles) {
            var distance = 0;
            foreach (var targetTile in targetTiles) {
                var distanceToEnemy = GridManager.Instance.GetDistance(tile, targetTile.Tile);
                if (distanceToEnemy > distance) distance = distanceToEnemy;
            }

            return distance;
        }
        
        private static bool IsFlanked(Tile tile, List<TargetTiles> enemyUnits) {
            var isFlanked = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(enemyUnit.Tile, tile);
                if (coverType == CoverType.None) isFlanked = true;
            }

            return isFlanked;
        }

        private static List<Unit> GetEnemiesInSight() {
            var squad = BattleManager.Instance.SquadTurn;
            var knownUnits = new List<Unit>();
            foreach (var unit in squad.ActiveUnits) {
                unit.GetEnemiesInLineOfSight().ForEach(u => knownUnits.Add(u));
            }
            
            return knownUnits;
        }
        
        private static bool CanFlank(Tile tile, List<TargetTiles> enemyUnits) {
            var canFlank = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(tile, enemyUnit.Tile);
                if (coverType == CoverType.None) canFlank = true;
            }

            return canFlank;
        }
        
        private static bool HeightAdvantage(Tile tile, List<TargetTiles> enemyUnits) {
            var heightAdvantage = false;
            foreach (var enemyUnit in enemyUnits) {
                if (tile.GridPosition.Y > enemyUnit.Tile.GridPosition.Y) heightAdvantage = true;
            }

            return heightAdvantage;
        }
        
        private static Dictionary<Tile, CoverType> GetTileCoverFromEnemyTiles(Tile tile, List<TargetTiles> enemyUnits) {
            var cover = new Dictionary<Tile, CoverType>();
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(enemyUnit.Tile, tile);
                cover.TryAdd(enemyUnit.Tile, coverType);
            }

            return cover;
        }
    }
    
    public struct TargetTiles {
        public Tile Tile;
        public Unit Unit;
        public TargetTileType Type;
        
        public enum TargetTileType {
            VisibleEnemy,
            LastSeenEnemy,
            Search
        }
    }
}