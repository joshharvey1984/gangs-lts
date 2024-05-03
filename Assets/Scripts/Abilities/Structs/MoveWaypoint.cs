using System.Collections.Generic;
using Gangs.Grid;

namespace Gangs.Abilities.Structs {
    public struct MoveWaypoint {
        public List<Tile> DirectPathTiles;
        public List<Tile> Tiles;
        public int Cost;
    }
}