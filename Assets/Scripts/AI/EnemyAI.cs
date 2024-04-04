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
            if (GameManager.Instance.SquadTurn is not AISquad) {
                Debug.LogError("EnemyAI.TakeTurn() called on a non-AI squad turn!");
                return;
            }
            
            var moveAbility = unit.Abilities[0] as MoveAbility;
            var fireAbility = unit.Abilities[1] as FireAbility;
            
            var currentTile = GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            
            var lastSeenEnemies = GameManager.Instance.SquadTurn.EnemyLastSeen;
            var enemiesInSight = GetEnemiesInSight();
            
            var enemyData = new List<EnemyUnits>();
            
            foreach (var (enemy, tile) in lastSeenEnemies) {
                var isVisible = enemiesInSight.Contains(enemy);
                enemyData.Add(new EnemyUnits {
                    Unit = enemy,
                    Tile = tile,
                    IsVisible = isVisible
                });
            }
            
            var squad = GameManager.Instance.SquadTurn as AISquad;
            var weightings = squad!.Weightings;
            
            var moveRange = moveAbility!.CalculateMoveRange();
            var bestMoveTile = FindBestTile(unit, moveRange, enemyData);
            var bestMoveTileValue = GetTilePointValue(bestMoveTile, enemyData, weightings);
            var currentTileValue = GetTilePointValue(currentTile, enemyData, weightings);
            
            if (bestMoveTileValue <= currentTileValue * 1.25f) {
                if (unit.GetEnemiesInLineOfSight().Count > 0) {
                    var target = unit.GetEnemiesInLineOfSight().First();
                    var targetTile = GridManager.Instance.Grid.FindGridUnit(target.GridUnit);
                    fireAbility!.Select();
                    fireAbility!.LeftClickTile(targetTile);
                }
                else {
                    GameManager.Instance.SquadTurn.EndUnitTurn();
                }
                
                return;
            }
            
            moveAbility!.AddWaypoint(bestMoveTile);
            moveAbility!.Execute();
        }
        
        private static Tile FindBestTile(Unit unit, List<MoveRange> moveRange, List<EnemyUnits> enemyUnits) {
            var squad = GameManager.Instance.SquadTurn as AISquad;
            var weightings = squad!.Weightings;
            
            var candidateMoves = new Dictionary<Tile, float>();
            foreach (var move in moveRange) {
                foreach (var tile in move.Tiles) {
                    if (tile.GridUnit != null) continue;
                    var pointValue = GetTilePointValue(tile, enemyUnits, weightings);
                    if (!candidateMoves.TryAdd(tile, pointValue)) candidateMoves[tile] += pointValue;
                }
            }
            
            if (candidateMoves.Count == 0) return GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            var bestMove = candidateMoves.OrderByDescending(kvp => kvp.Value).First();
            return bestMove.Key;
        }

        private static float GetTilePointValue(Tile tile, List<EnemyUnits> enemyUnits, EnemyAIWeightings weightings) {
            var pointValue = 0f;
            var cover = GetTileCoverFromEnemyTiles(tile, enemyUnits);
            if (cover is {Count: > 0}){
                var worstCover = cover.Values.Min();
                if (worstCover == CoverType.Half) pointValue += weightings.HalfCoverWeight;
                if (worstCover == CoverType.Full) pointValue += weightings.FullCoverWeight;
            }
            
            if (HeightAdvantage(tile, enemyUnits)) pointValue += weightings.HeightAdvantageWeight;
            if (CanFlank(tile, enemyUnits)) pointValue += weightings.CanFlankWeight;
            if (IsFlanked(tile, enemyUnits)) pointValue += weightings.IsFlankedWeight;
            
            return pointValue;
        }
        
        private static bool IsFlanked(Tile tile, List<EnemyUnits> enemyUnits) {
            var isFlanked = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(enemyUnit.Tile, tile);
                if (coverType == CoverType.None) isFlanked = true;
            }

            return isFlanked;
        }

        private static List<Unit> GetEnemiesInSight() {
            var squad = GameManager.Instance.SquadTurn;
            var knownUnits = new List<Unit>();
            foreach (var unit in squad.Units) {
                unit.GetEnemiesInLineOfSight().ForEach(u => knownUnits.Add(u));
            }
            
            return knownUnits;
        }
        
        private static bool CanFlank(Tile tile, List<EnemyUnits> enemyUnits) {
            var canFlank = false;
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(tile, enemyUnit.Tile);
                if (coverType == CoverType.None) canFlank = true;
            }

            return canFlank;
        }
        
        private static bool HeightAdvantage(Tile tile, List<EnemyUnits> enemyUnits) {
            var heightAdvantage = false;
            foreach (var enemyUnit in enemyUnits) {
                if (tile.GridPosition.Y > enemyUnit.Tile.GridPosition.Y) heightAdvantage = true;
            }

            return heightAdvantage;
        }
        
        private static Dictionary<Tile, CoverType> GetTileCoverFromEnemyTiles(Tile tile, List<EnemyUnits> enemyUnits) {
            var cover = new Dictionary<Tile, CoverType>();
            foreach (var enemyUnit in enemyUnits) {
                var coverType = GridManager.Instance.CheckTileCover(enemyUnit.Tile, tile);
                cover.TryAdd(enemyUnit.Tile, coverType);
            }

            return cover;
        }
    }
    
    public struct EnemyUnits {
        public Unit Unit;
        public Tile Tile;
        public bool IsVisible;
    }
}