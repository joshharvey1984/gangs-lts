using System;
using System.Collections.Generic;
using Gangs.Managers;

namespace Gangs.Grid {
    public static class Pathfinder {
        public static Grid Grid = GridManager.Instance.Grid;

        public static Dictionary<Tile, int> CalculateMoveRange(Tile startPosition, int movementPoints) {
            var reachableTiles = new Dictionary<Tile, int>();
            var tiles = Grid.Tiles;

            var maxMove = movementPoints / 10;

            var startX = Math.Max(0, startPosition.GridPosition.X - maxMove);
            var endX = Math.Min(tiles.GetLength(0), startPosition.GridPosition.X + maxMove);

            var startY = Math.Max(0, startPosition.GridPosition.Y - maxMove);
            var endY = Math.Min(tiles.GetLength(1), startPosition.GridPosition.Y + maxMove);

            var startZ = Math.Max(0, startPosition.GridPosition.Z - maxMove);
            var endZ = Math.Min(tiles.GetLength(2), startPosition.GridPosition.Z + maxMove);

            for (var x = startX; x <= endX; x++) {
                for (var y = startY; y <= endY; y++) {
                    for (var z = startZ; z <= endZ; z++) {
                        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1) || z < 0 || z >= tiles.GetLength(2)) continue;
                        var endPosition = Grid.Tiles[x, y, z];
                        if (endPosition == null) continue;

                        var path = FindPath(startPosition, endPosition);

                        if (path != null) {
                            var pathCost = CalculatePathCost(path);
                            if (pathCost <= movementPoints) {
                                reachableTiles.Add(tiles[x, y, z], pathCost);
                            }
                        }
                    }
                }
            }

            return reachableTiles;
        }

        private static List<Tile> FindPath(Tile start, Tile end) {
            var openSet = new List<Tile>();
            var closedSet = new HashSet<Tile>();
            var cameFrom = new Dictionary<Tile, Tile>();
            var gScore = new Dictionary<Tile, int>();
            var fScore = new Dictionary<Tile, int>();

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = GetDistance(start, end);

            while (openSet.Count > 0) {
                var current = openSet[0];
                for (var i = 1; i < openSet.Count; i++) {
                    if (fScore[openSet[i]] < fScore[current] || !fScore.ContainsKey(current)) {
                        current = openSet[i];
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == end) {
                    return RetracePath(cameFrom, start, end);
                }

                foreach (var neighbor in Grid.GetValidNeighbours(current)) {
                    if (closedSet.Contains(neighbor)) {
                        continue;
                    }

                    var movementCost = GetDistance(current, neighbor);
                    var tentativeGScore = gScore[current] + movementCost;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore >= gScore[neighbor]) {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + GetDistance(neighbor, end);
                }
            }

            return null;
        }
        
        public static PathData FindOptimizedPath(Tile start, Tile end) {
            var path = FindPath(start, end);

            var sections = new List<List<Tile>>();
            var currentSection = new List<Tile> { path[0] };
            for (var i = 1; i < path.Count; i++) {
                if (path[i].GridPosition.Y != path[i - 1].GridPosition.Y) {
                    if (currentSection.Count > 0) {
                        sections.Add(currentSection);
                    }
                    currentSection = new List<Tile>();
                }
                currentSection.Add(path[i]);
            }

            if (currentSection.Count > 0) {
                sections.Add(currentSection);
            }

            var pathData = new PathData {
                PathTiles = new List<Tile>(),
                DirectPathTiles = new List<Tile>(),
                Cost = 0
            };
    
            foreach (var section in sections) {
                if (section.Count == 1) {
                    pathData.PathTiles.Add(section[0]);
                    pathData.DirectPathTiles.Add(section[0]);
                    pathData.Cost += CalculatePathCost(new List<Tile> {section[0]});
                }
                else {
                    var optimizedPath = LerpPath(section);
                    pathData.PathTiles.AddRange(optimizedPath.PathTiles);
                    pathData.DirectPathTiles.AddRange(optimizedPath.DirectPathTiles);
                    pathData.Cost += optimizedPath.Cost;
                }
            }
    
            return pathData;
        }

        private static PathData LerpPath(List<Tile> path) {
            var pathData = new PathData {
                PathTiles = new List<Tile> { path[0] },
                DirectPathTiles = new List<Tile> { path[0] }
            };
            
            var currentTile = path[0];
            var checkTile = path[^1];

            while (currentTile != path[^1]) {
                var line = GridUtils.GetAllTilesInStraightLine(Grid, currentTile, checkTile);
                if (IsPathClear(line)) {
                    pathData.PathTiles.AddRange(line);
                    pathData.DirectPathTiles.Add(line[^1]);
                    pathData.Cost += CalculatePathCost(line);
                    currentTile = checkTile;
                    checkTile = path[^1];
                }
                else {
                    checkTile = path[path.IndexOf(checkTile) - 1];
                }
            }
            
            return pathData;
        }


        private static bool IsPathClear(List<Tile> path) {
            for (var i = 1; i < path.Count; i++) {
                if (!Grid.CanMoveFromTileToTile(path[i - 1], path[i])) {
                    return false;
                }
            }
            return true;
        }

        private static int CalculatePathCost(List<Tile> path) {
            var pathCost = 0;
            for (var i = 1; i < path.Count; i++) {
                pathCost += GetDistance(path[i - 1], path[i]);
            }

            return pathCost;
        }

        private static List<Tile> RetracePath(Dictionary<Tile, Tile> cameFrom, Tile start, Tile end) {
            var path = new List<Tile>();
            var current = end;

            while (current != start) {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        private static int GetDistance(Tile nodeA, Tile nodeB) {
            var dstX = Math.Abs(nodeA.GridPosition.X - nodeB.GridPosition.X);
            var dstY = Math.Abs(nodeA.GridPosition.Y - nodeB.GridPosition.Y);
            var dstZ = Math.Abs(nodeA.GridPosition.Z - nodeB.GridPosition.Z);

            int cost;
            if (dstX > dstZ) {
                cost = 14 * dstZ + 10 * (dstX - dstZ);
            }
            else {
                cost = 14 * dstX + 10 * (dstZ - dstX);
            }

            cost += 10 * dstY;

            return cost;
        }
    }
    
    public struct PathData {
        public List<Tile> PathTiles;
        public List<Tile> DirectPathTiles;
        public int Cost;
    }
}
