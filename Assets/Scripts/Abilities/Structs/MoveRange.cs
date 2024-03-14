using System.Collections.Generic;
using Gangs.Grid;

namespace Gangs.Abilities.Structs {
    public struct MoveRange {
        public readonly int ActionPoint;
        public readonly List<Tile> Tiles;

        public MoveRange(int actionPoint, List<Tile> tiles) {
            Tiles = tiles;
            ActionPoint = actionPoint;
        }
    }
}