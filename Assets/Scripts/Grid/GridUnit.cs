using System;

namespace Gangs.Grid {
    public class GridUnit {
        public event Func<GridUnit, Tile> OnTileGetPosition;
        public Tile GetTile() => OnTileGetPosition?.Invoke(this);
    }
}
