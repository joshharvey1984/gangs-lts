using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Core;
using Gangs.Grid;

namespace Gangs.Abilities {
    public static class AbilityTargetingExtensions {
        public static List<TargetTiles> GetTargetingTiles(this TargetingType targetingType, Tile fromTile, BattleUnit battleUnit, BattleGrid battleGrid) {
            return targetingType switch {
                TargetingType.StandardMove => StandardMove(fromTile, battleUnit),
                TargetingType.EnemiesInLineOfSight => EnemiesInLineOfSight(fromTile, battleGrid),
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
        
        private static List<TargetTiles> EnemiesInLineOfSight(Tile fromTile, BattleGrid battleGrid) {
            var losPositions = fromTile.LineOfSightGridPositions;
            var losTiles = losPositions.Select(p => battleGrid.Grid.GetTile(p)).ToList();
            return new List<TargetTiles> { new() { Tiles = losTiles, Cost = 1 } };
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