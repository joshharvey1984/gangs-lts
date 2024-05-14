using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Core;
using Gangs.Grid;

namespace Gangs.Battle.AI {
    public static class BattleAI {
        public static BattleBase BattleBase { private get; set; }
        public static void TakeTurn() {
            if (BattleBase is null) throw new Exception("BattleAI.Battle is null!");
            if (BattleBase.ActiveSquad is null) throw new Exception("BattleAI.Battle.ActiveSquad is null!");
            
            var activeSquad = BattleBase.ActiveSquad;
            var battleUnit = activeSquad.SelectedUnit;
            
            var moveAbility = battleUnit.Abilities[0] as MoveAbility;
            var fireAbility = battleUnit.Abilities[1] as FireAbility;
            
            var currentTile = battleUnit.GridUnit.GetTile();
            
            var lastSeenEnemies = activeSquad.EnemyLastSeen;
            var enemiesInSight = GetEnemiesInSquadSight(activeSquad);
            
            var enemyData = new List<SearchTiles>();
            
            foreach (var (enemy, tile) in lastSeenEnemies) {
                enemyData.Add(new SearchTiles {
                    BattleUnit = enemy,
                    Tile = tile,
                    Type = enemiesInSight.Contains(enemy) 
                        ? SearchTiles.SearchTileType.VisibleEnemy 
                        : SearchTiles.SearchTileType.LastSeenEnemy
                });
            }
            
            if (enemyData.Count == 0) {
                var centreTile = BattleBase.Grid.Grid.GetRandomCenterTile();
                enemyData.Add(new SearchTiles {
                    BattleUnit = null,
                    Tile = centreTile,
                    Type = SearchTiles.SearchTileType.Search
                });
            }
            
            var weightings = GetWeightings(battleUnit);
            var moveRange = moveAbility!.TargetingType.GetTargetingTiles(currentTile, battleUnit, BattleBase);
            var bestMoveTiles = FindBestTile(moveRange, enemyData, weightings);
            var bestMove = bestMoveTiles.Take(3).ElementAt(new Random().Next(0, 3));
            
            var bestMoveTileValue = bestMove.Value;
            var currentTileValue = GetTilePointValue(currentTile, enemyData, weightings, battleUnit.ActionPointsRemaining);
            
            if (bestMoveTileValue <= currentTileValue + 1) {
                if (GetEnemiesInLineOfSight(battleUnit).ToList().Count > 0) {
                    var target = GetEnemiesInLineOfSight(battleUnit).First();
                    var targetTile = target.GridUnit.GetTile();
                    fireAbility!.Select();
                    fireAbility!.OnAbilityFinished += AbilityFinished;
                    fireAbility!.SetTarget(targetTile);
                    return;
                }
            }
            
            moveAbility!.OnAbilityFinished += AbilityFinished;
            moveAbility!.SetTarget(bestMove.Key);
        }
        
        private static void AbilityFinished() {
            BattleBase.ActiveSquad.EndUnitTurn();
        }
        
        private static Dictionary<Tile, float> FindBestTile(List<TargetTiles> moveRange, List<SearchTiles> targetTiles, BattleAIWeightings weightings) {
            var candidateMoves = new Dictionary<Tile, float>();
            foreach (var move in moveRange) {
                foreach (var tile in move.Tiles) {
                    if (tile.GridUnit != null) continue;
                    var pointValue = GetTilePointValue(tile, targetTiles, weightings, move.Cost);
                    if (!candidateMoves.TryAdd(tile, pointValue)) candidateMoves[tile] += pointValue;
                }
            }
            
            return candidateMoves.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static float GetTilePointValue(Tile tile, List<SearchTiles> targetTiles, BattleAIWeightings weightings, int apRemaining) {
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
        
        private static int DistanceCheck(Tile tile, List<SearchTiles> targetTiles) {
            var distance = 0;
            foreach (var targetTile in targetTiles) {
                var distanceToEnemy = BattleBase.Grid.Grid.GetDistance(tile, targetTile.Tile);
                if (distanceToEnemy > distance) distance = distanceToEnemy;
            }

            return distance;
        }
        
        private static bool IsFlanked(Tile tile, List<SearchTiles> enemyUnits) {
            var isFlanked = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = BattleBase.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
                if (coverType == CoverType.None) isFlanked = true;
            }

            return isFlanked;
        }

        private static List<BattleUnit> GetEnemiesInSquadSight(BattleSquad activeSquad) => 
            activeSquad.ActiveUnits.SelectMany(GetEnemiesInLineOfSight).ToList();

        private static IEnumerable<BattleUnit> GetEnemiesInLineOfSight(BattleUnit battleUnit) {
            var unitTile = battleUnit.GridUnit.GetTile();
            var losUnits = BattleBase.Grid.GetUnitsInSightOfTile(unitTile, 20);
            var enemyUnits = BattleBase.GetActiveEnemyUnits(battleUnit);
            return losUnits.Where(unit => enemyUnits.Contains(unit)).ToList();
        }
        
        private static bool CanFlank(Tile tile, List<SearchTiles> enemyUnits) {
            var canFlank = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = BattleBase.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
                if (coverType == CoverType.None) canFlank = true;
            }

            return canFlank;
        }
        
        private static bool HeightAdvantage(Tile tile, List<SearchTiles> enemyUnits) {
            var heightAdvantage = false;
            foreach (var enemyUnit in enemyUnits) {
                if (tile.GridPosition.Y > enemyUnit.Tile.GridPosition.Y) heightAdvantage = true;
            }

            return heightAdvantage;
        }
        
        private static Dictionary<Tile, CoverType> GetTileCoverFromEnemyTiles(Tile tile, List<SearchTiles> enemyUnits) {
            var cover = new Dictionary<Tile, CoverType>();
            foreach (var enemyUnit in enemyUnits) {
                var coverType = BattleBase.Grid.GetCoverType(tile.GridPosition, enemyUnit.Tile.GridPosition);
                cover.TryAdd(enemyUnit.Tile, coverType);
            }

            return cover;
        }
        
        private static BattleAIWeightings GetWeightings(BattleUnit battleUnit) {
            return new BattleAIWeightings {
                HalfCoverWeight = 1,
                FullCoverWeight = 2,
                HeightAdvantageWeight = 1,
                CanFlankWeight = 2,
                IsFlankedWeight = 2,
                DistanceCheckWeight = 1,
                RemainingActionPointWeight = 1
            };
        }
    }
    
    public struct SearchTiles {
        public Tile Tile;
        public BattleUnit BattleUnit;
        public SearchTileType Type;
        
        public enum SearchTileType {
            VisibleEnemy,
            LastSeenEnemy,
            Search
        }
    }
}