using System;
using System.Collections.Generic;

namespace Gangs.Grid {
    [Serializable]
    public class Tile {
        public GridUnit GridUnit { get; set; }
        public GridPosition GridPosition { get; }
        public Prop Prop { get; set; }
        public Dictionary<CardinalDirection, Wall> Walls { get; } = new();
        public Dictionary<CardinalDirection, Prop> NeighbourProps { get; } = new();
        public Climbable Climbable { get; set; }
        public HashSet<GridPosition> LineOfSightGridPositions { get; } = new();

        public Tile(GridPosition gridPosition) {
            GridPosition = gridPosition;
        }
        
        public override string ToString() {
            return $"Tile at {GridPosition}";
        }
    }
}
