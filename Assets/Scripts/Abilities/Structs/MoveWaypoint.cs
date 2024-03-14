using System.Collections.Generic;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Abilities.Structs {
    public struct MoveWaypoint {
        public List<Tile> DirectPathTiles;
        public List<Tile> Tiles;
        public int Cost;
        public GameObject Indicator;
            
        public void DestroyIndicator() {
            if (Indicator != null) Object.Destroy(Indicator);
        }
    }
}