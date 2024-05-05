using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
using Gangs.Core;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Battle.AI {
    public static class BattleAI {
        private static Battle _battle;
        public static void TakeTurn(BattleUnit battleUnit, Battle battle) {
            _battle = battle;
            if (battle.ActiveSquad is not AIBattleSquad activeSquad) {
                Debug.LogError("EnemyAI.TakeTurn() called on a non-AI squad turn!");
                return;
            }
            
            var moveAbility = battleUnit.Abilities[0] as MoveAbility;
            var fireAbility = battleUnit.Abilities[1] as FireAbility;
            
            var currentTile = battleUnit.GridUnit.GetTile();
            
            var lastSeenEnemies = activeSquad.EnemyLastSeen;
            var enemiesInSight = GetEnemiesInSquadSight(activeSquad);
            
            var enemyData = new List<TargetTiles>();
            
            foreach (var (enemy, tile) in lastSeenEnemies) {
                enemyData.Add(new TargetTiles {
                    BattleUnit = enemy,
                    Tile = tile,
                    Type = enemiesInSight.Contains(enemy) 
                        ? TargetTiles.TargetTileType.VisibleEnemy 
                        : TargetTiles.TargetTileType.LastSeenEnemy
                });
            }
            
            if (enemyData.Count == 0) {
                var centreTile = battle.Grid.Grid.GetRandomCenterTile();
                enemyData.Add(new TargetTiles {
                    BattleUnit = null,
                    Tile = centreTile,
                    Type = TargetTiles.TargetTileType.Search
                });
            }
            
            var weightings = activeSquad!.Weightings;
            
            var moveRange = moveAbility!.CalculateMoveRange();
            var bestMoveTiles = FindBestTile(battleUnit, moveRange, enemyData, activeSquad);
            var bestMove = bestMoveTiles.Take(3).ElementAt(Random.Range(0, 3));
            
            var bestMoveTileValue = bestMove.Value;
            var currentTileValue = GetTilePointValue(currentTile, enemyData, weightings, battleUnit.ActionPointsRemaining);
            
            if (bestMoveTileValue <= currentTileValue) {
                if (GetEnemiesInLineOfSight(battleUnit).ToList().Count > 0) {
                    var target = GetEnemiesInLineOfSight(battleUnit).First();
                    var targetTile = target.GridUnit.GetTile();
                    fireAbility!.Select();
                    fireAbility!.OnAbilityFinished += AbilityFinished;
                    fireAbility!.LeftClickTile(targetTile);
                    return;
                }
            }
            
            moveAbility!.AddWaypoint(bestMove.Key);
            moveAbility!.OnAbilityFinished += AbilityFinished;
            moveAbility!.Execute();
        }
        
        private static void AbilityFinished() {
            _battle.ActiveSquad.EndUnitTurn();
        }
        
        private static Dictionary<Tile, float> FindBestTile(BattleUnit battleUnit, List<MoveRange> moveRange, List<TargetTiles> targetTiles, AIBattleSquad activeSquad) {
            var weightings = activeSquad!.Weightings;
            
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

        private static float GetTilePointValue(Tile tile, List<TargetTiles> targetTiles, BattleAIWeightings weightings, int apRemaining) {
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
                var distanceToEnemy = _battle.Grid.Grid.GetDistance(tile, targetTile.Tile);
                if (distanceToEnemy > distance) distance = distanceToEnemy;
            }

            return distance;
        }
        
        private static bool IsFlanked(Tile tile, List<TargetTiles> enemyUnits) {
            var isFlanked = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = _battle.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
                if (coverType == CoverType.None) isFlanked = true;
            }

            return isFlanked;
        }

        private static List<BattleUnit> GetEnemiesInSquadSight(AIBattleSquad activeSquad) => 
            activeSquad.ActiveUnits.SelectMany(GetEnemiesInLineOfSight).ToList();

        private static IEnumerable<BattleUnit> GetEnemiesInLineOfSight(BattleUnit battleUnit) {
            var unitTile = battleUnit.GridUnit.GetTile();
            var losUnits = _battle.Grid.GetUnitsInSightOfTile(unitTile, 20);
            var enemyUnits = _battle.GetEnemyUnits(battleUnit);
            return losUnits.Where(unit => enemyUnits.Contains(unit)).ToList();
        }
        
        private static bool CanFlank(Tile tile, List<TargetTiles> enemyUnits) {
            var canFlank = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = _battle.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
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
                var coverType = _battle.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
                cover.TryAdd(enemyUnit.Tile, coverType);
            }

            return cover;
        }
    }
    
    public struct TargetTiles {
        public Tile Tile;
        public BattleUnit BattleUnit;
        public TargetTileType Type;
        
        public enum TargetTileType {
            VisibleEnemy,
            LastSeenEnemy,
            Search
        }
    }
}