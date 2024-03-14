using System;
using System.Collections.Generic;

namespace Gangs.Grid {
    public static class GridUtils {
        public static List<Tile> GetAllTilesInStraightLine(Grid grid, Tile p0, Tile p1) {
            var points = new List<Tile>();
            var n = DiagonalDistance(p0.GridPosition, p1.GridPosition);
            for (var step = 0; step <= n; step++) {
                var t = n == 0 ? 0.0f : (float) step / n;
                var pos = LerpPoint(p0.GridPosition, p1.GridPosition, t);
                points.Add(grid.Tiles[pos.X, pos.Y, pos.Z]);
            }

            return points;
        }

        public static List<GridPosition> GetAllGridPositionsInStraightLine(Grid grid, GridPosition g0, GridPosition g1) {
            var points = new List<GridPosition>();
            var n = DiagonalDistance(g0, g1);
            for (var step = 0; step <= n; step++) {
                var t = n == 0 ? 0.0f : (float) step / n;
                var pos = LerpPoint(g0, g1, t);
                points.Add(pos);
            }

            return points;
        }
        
        private static int DiagonalDistance(GridPosition p0, GridPosition p1) {
            var dx = p1.X - p0.X;
            var dz = p1.Z - p0.Z;
            return Math.Max(Math.Abs(dx), Math.Abs(dz));
        }

        private static GridPosition LerpPoint(GridPosition p0, GridPosition p1, float t) {
            return new GridPosition(
                Lerp(p0.X, p1.X, t),
                p0.Y,
                Lerp(p0.Z, p1.Z, t)
            );
        }

        private static int Lerp(int start, int end, float t) => (int) Math.Round(start * (1.0f - t) + t * end);
        
        public static List<GridPosition> GetPositionsInArc(Grid grid, GridPosition origin, CardinalDirection direction, int range) {
            var positions = new List<GridPosition>();
            var returnPositions = new List<GridPosition>();

            // get all positions on grid within range
            var allPositions = grid.GetGridPositionsInRangeByLevelOffset(origin, range); 
            allPositions.ForEach(t => positions.Add(t));

            foreach (var position in positions) {
                var dir = position - origin;
                if (dir.X == 0 && dir.Z == 0) continue;
                var dir2D = new Direction2D(dir.X, dir.Z).ToCardinalDirection();
                if (dir2D == direction) returnPositions.Add(position);
            }

            return returnPositions;
        }
        
        public static Dictionary<Tile, float> GetAllTilesInSupercoverLine(Grid grid, Tile p0, Tile p1) 
        {
            var result = new Dictionary<Tile, float>();
            var p = new GridPosition(p0.GridPosition.X, p0.GridPosition.Y, p0.GridPosition.Z);
            var dx = p1.GridPosition.X - p0.GridPosition.X;
            var dy = p1.GridPosition.Z - p0.GridPosition.Z;
            var nx = Math.Abs(dx);
            var ny = Math.Abs(dy);
            var signX = dx > 0 ? 1 : -1;
            var signY = dy > 0 ? 1 : -1;

            // Calculate initial intersection percentage based on tile diagonal
            var percentage = 1f;
            result.Add(grid.GetTile(p), percentage);

            for (int ix = 0, iy = 0; ix < nx || iy < ny;) 
            {
                var decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
                if (decision == 0) 
                {
                    // Next step is diagonal
                    p = new GridPosition(p.X + signX, p.Y, p.Z + signY);
                    ix++;
                    iy++;
                    percentage = 1f;  // Full coverage if line passes through diagonal
                } 
                else if (decision < 0) 
                {
                    // Next step is horizontal
                    p = new GridPosition(p.X + signX, p.Y, p.Z);
                    ix++;
                    percentage = 0.5f;  // 50% coverage if line enters from the side
                } 
                else 
                {
                    // Next step is vertical
                    p = new GridPosition(p.X, p.Y, p.Z + signY);
                    iy++;
                    percentage = 0.5f;  // 50% coverage if line enters from the side
                }
                result.Add(grid.GetTile(p), percentage);
            }
            return result;
        }
    }
}