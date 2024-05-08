using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Core;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs.Abilities {
    public static class AbilityTargetingExtensions {
        public static List<TargetTiles> GetTargetingTiles(this TargetingType targetingType, Tile fromTile, BattleUnit battleUnit) {
            switch (targetingType) {
                case TargetingType.StandardMove:
                    return StandardMove(fromTile, battleUnit);
                
                case TargetingType.EnemiesInLineOfSight:
                    return EnemiesInLineOfSight(fromTile);
                
                default:
                    return new List<TargetTiles>();
            }
        }
        
        private static List<TargetTiles> StandardMove(Tile fromTile, BattleUnit battleUnit) {
            var targetTiles = new List<TargetTiles>();
            var moveRange = Pathfinder.CalculateMoveRange(fromTile, battleUnit.GetAttributeValue(UnitAttributeType.Movement) * 10);
            
            for (var i = 0; i < battleUnit.ActionPointsRemaining; i++) {
                var apMoveRange = battleUnit.GetAttributeValue(UnitAttributeType.Movement) * 10 * (i + 1);
                var m = moveRange.Where(kvp => kvp.Value <= apMoveRange);
                var keys = m.Select(kvp => kvp.Key).ToList();
                var moveRangeList = new TargetTiles { Tiles = keys, Cost = i };
                targetTiles.Add(moveRangeList);
            }
            
            return targetTiles;
        }
        
        private static List<TargetTiles> EnemiesInLineOfSight(Tile fromTile) {
            var losPositions = fromTile.LineOfSightGridPositions;
            var losTiles = losPositions.Select(pos => 
                BattleGridManager.Instance.GetTile(pos)).Where(tile => tile != null).ToList();
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