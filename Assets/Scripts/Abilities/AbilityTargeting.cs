using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Core;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs.Abilities {
    public static class AbilityTargetingExtensions {
        public static List<TargetTiles> GetTargetingTiles(this TargetingType targetingType, Tile fromTile, BattleUnit battleUnit, BattleBase battle) {
            return targetingType switch {
                TargetingType.StandardMove => StandardMove(fromTile, battleUnit),
                TargetingType.EnemiesInLineOfSight => EnemiesInLineOfSight(fromTile, battleUnit, battle),
                _ => new List<TargetTiles>()
            };
        }
        
        private static List<TargetTiles> StandardMove(Tile fromTile, BattleUnit battleUnit) {
            var targetTiles = new List<TargetTiles>();
            var moveRange = Pathfinder.CalculateMoveRange(fromTile, 
                battleUnit.GetTotalMovePoints());
            
            for (var i = 0; i < battleUnit.ActionPointsRemaining; i++) {
                var apMoveRange = battleUnit.GetAttributeValue(UnitAttributeType.Movement) * 10 * (i + 1);
                var m = moveRange.Where(kvp => kvp.Value <= apMoveRange);
                var keys = m.Select(kvp => kvp.Key).ToList();
                var moveRangeList = new TargetTiles { Tiles = keys, Cost = i + 1 };
                targetTiles.Add(moveRangeList);
            }
            
            return targetTiles;
        }
        
        private static List<TargetTiles> EnemiesInLineOfSight(Tile fromTile, BattleUnit battleUnit, BattleBase battle) {
            var losPositions = fromTile.LineOfSightGridPositions;
            var enemies = battle.GetActiveEnemyUnits(battleUnit);
            var targetTiles = new List<TargetTiles>();
            
            foreach (var enemy in enemies) {
                var enemyTile = enemy.GridUnit.GetTile();
                if (losPositions.Contains(enemyTile.GridPosition)) {
                    targetTiles.Add(new TargetTiles { Tiles = new List<Tile> { enemyTile }, Cost = 1 });
                }
            }
            
            return targetTiles;
        }
    }
    
    public enum TargetingType {
        StandardMove,
        EnemiesInLineOfSight,
    }
    
    public struct TargetTiles {
        public List<Tile> Tiles;
        public int Cost;
    }
}