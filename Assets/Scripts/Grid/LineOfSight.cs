using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gangs.Managers;

namespace Gangs.Grid {
    public static class LineOfSight {
        private static readonly Grid Grid = GridManager.Instance.Grid;
        
        public static async void BuildLineOfSightData() {
            var tasks = new List<Task>();

            foreach (var tile in Grid.Tiles) {
                if (tile == null) continue;
                tasks.Add(Task.Run(() => {
                    var losPositions = GetLineOfSightTiles(tile.GridPosition, 20);
                    lock (tile.LineOfSightGridPositions) {
                        tile.LineOfSightGridPositions.UnionWith(losPositions);
                    }
                    var differentLevelPositions = losPositions.Where(t => t.Y != tile.GridPosition.Y);
                    foreach (var losPosition in differentLevelPositions) {
                        var losTile = Grid.GetTile(losPosition);
                        if (losTile == null) continue;
                        lock (losTile.LineOfSightGridPositions) {
                            losTile.LineOfSightGridPositions.Add(tile.GridPosition);
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            
            //make sure all tiles have two way line of sight
            foreach (var tile in Grid.Tiles) {
                if (tile == null) continue;
                foreach (var losPosition in tile.LineOfSightGridPositions) {
                    Grid.GetTile(losPosition)?.LineOfSightGridPositions.Add(tile.GridPosition);
                }
            }
            
        }


        private static List<GridPosition> GetLineOfSightTiles(GridPosition gridPosition, int range, bool checkingCorner = false) {
            var gridPositions = Grid.GetGridPositionsInRangeByLevelOffset(gridPosition, range);
            gridPositions.Remove(gridPosition);

            gridPositions.Sort((a, b) => {
                var distanceA = GridPosition.Distance(gridPosition, a);
                var distanceB = GridPosition.Distance(gridPosition, b);
                return distanceA.CompareTo(distanceB);
            });

            // starting with the tile closest to the tile, check if there's a clear line of site
            // if there is, add it to the list of tiles in line of site
            // if there isn't, add it to blocked list
            var blocked = new List<GridPosition>();
            var lineOfSite = new List<GridPosition>();

            foreach (var checkPosition in gridPositions) {
                var lineTiles = GridUtils.GetAllGridPositionsInStraightLine(Grid, gridPosition, checkPosition);
                
                // if any of the tiles in the line are blocked, then the line is blocked
                if (lineTiles.Any(blocked.Contains)) continue;
                
                for (var i = 0; i < lineTiles.Count - 1; i++) {
                    if (Grid.CanSeeFromPositionToPosition(lineTiles[i], lineTiles[i + 1])) {
                        lineOfSite.Add(lineTiles[i + 1]);
                    }
                    else {
                        blocked.Add(lineTiles[i + 1]);
                        break;
                    }
                }
            }
            
            var aboveTiles = AboveTiles(gridPosition, range);
            lineOfSite.AddRange(aboveTiles);
            
            var returnTiles = new List<Tile>();
            foreach (var position in lineOfSite) {
                returnTiles.Add(Grid.GetTile(position));
            }
            
            // if tile is corner tile, then get the line of sight for the tile around the corner
            if (checkingCorner == false && Grid.GetTile(gridPosition) != null) {
                //check four directions for wall
                foreach (var kvp in Grid.GetTile(gridPosition)!.Walls) {
                    var tile = Grid.GetTile(gridPosition);
                    // check there is no wall either side of this wall
                    var wallDirection = kvp.Key;
                    foreach (var adjacentDirection in wallDirection.GetFourAdjacent()) {
                        if (tile!.Walls.ContainsKey(adjacentDirection)) {
                            if (tile.Walls[adjacentDirection].LineOfSightBlocker) {
                                continue;
                            }
                        }
                        // get the tile in the direction of adjacent direction
                        var adjacentTile = Grid.GetTile(GridPosition.GridPositionFromDirection(tile.GridPosition, adjacentDirection));
                        if (adjacentTile != null && adjacentTile.Walls.ContainsKey(wallDirection)) continue;
                        // if there is no wall in the direction of the adjacent direction, then check the line of sight for the tile around the corner
                        var peakPos = GridPosition.GridPositionFromDirection(GridPosition.GridPositionFromDirection(tile.GridPosition, adjacentDirection), wallDirection);
                        if (!Grid.IsPositionWithinGridBounds(peakPos)) continue;
                        returnTiles.Add(Grid.GetTile(peakPos));
                        returnTiles.AddRange(GetLineOfSightTiles(peakPos, range, true).Select(Grid.GetTile));
                        returnTiles.AddRange(GetAllPositionsThatCanSeePosition(peakPos).Select(Grid.GetTile));
                    }
                }
            }
            
            // get all tiles where not null
            return returnTiles.Where(t => t != null).Select(t => t.GridPosition).ToList();
        }
        
        private static List<GridPosition> GetAllPositionsThatCanSeePosition(GridPosition gridPosition) {
            var positions = new List<GridPosition>();
            foreach (var tile in Grid.Tiles) {
                if (tile == null) continue;
                if (tile.LineOfSightGridPositions.Contains(gridPosition)) {
                    positions.Add(tile.GridPosition);
                }
            }
            
            return positions;
        }
        
        private static List<GridPosition> AboveTiles(GridPosition gridPosition, int range) {
            var tiles = new List<Tile>();
            
            // get number of levels above tile from grid array
            var numLevel = Grid.Tiles.GetLength(1);
            var level = gridPosition.Y;
            var levelsAbove = numLevel - level - 1;
            
            // if there are no levels above, return empty list
            if (levelsAbove == 0) return new List<GridPosition>();
            
            // if there are levels above, get all tiles in range on each level - the number of levels above
            for (var i = 1; i <= levelsAbove; i++) {
                var positionAbove = new GridPosition(gridPosition.X, gridPosition.Y + i, gridPosition.Z);
                var tileAbove = Grid.GetTile(positionAbove);
                var checkTilePositions = Grid.GetAllNeighbourPositions(positionAbove);
                foreach (var checkTilePosition in checkTilePositions) {
                    var checkTile = Grid.GetTile(checkTilePosition);
                    if (tileAbove != null && checkTile != null) continue;
                    
                    // check theres no wall or prop between the tile checkTilePosition.y 
                    if (Grid.CanSeeFromPositionToPosition(gridPosition, new GridPosition(checkTilePosition.X, gridPosition.Y, checkTilePosition.Z)) == false) continue;
                    
                    // if there's no tile there, then get the cardinal direction from the tile to the position above
                    var xDiff = checkTilePosition.X - gridPosition.X;
                    var zDiff = checkTilePosition.Z - gridPosition.Z;
                    var direction = new Direction2D(xDiff, zDiff).ToCardinalDirection();
                    
                    // get all positions in 45 degree arc in that direction
                    var arcPositions = GridUtils.GetPositionsInArc(Grid, gridPosition, direction, range);
                    
                    // add 1 to the y value of all positions
                    arcPositions = arcPositions.Select(p => new GridPosition(p.X, p.Y + 1, p.Z)).ToList();

                    // get all tiles in those positions
                    var arcTiles = arcPositions.Select(Grid.GetTile).ToList();
                    
                    // remove nulls
                    arcTiles.RemoveAll(t => t == null);

                    var edgeTile = new List<Tile>();
                    foreach (var arcTile in arcTiles) {
                        // get direction from tile to arcTile
                        var arcTileDirection = new Direction2D(arcTile.GridPosition.X - gridPosition.X, arcTile.GridPosition.Z - gridPosition.Z).ToCardinalDirection();
                        // check if there is a tile in the direction of the arcTile directly
                        var tileInDirection = Grid.GetTile(GridPosition.GridPositionFromDirection(arcTile.GridPosition, arcTileDirection.GetOpposite()));
                        if (tileInDirection != null) continue;
                        
                        // check there is no wall between the tile and the arcTile
                        // TODO: needs improving
                        if (arcTile.Walls.Any(w => w.Key == arcTileDirection)) continue;
                        
                        edgeTile.Add(arcTile);
                    }
                    
                    // add all tiles that are not null and are not the tile to the list of tiles
                    tiles.AddRange(edgeTile.Where(t => t != null && t != Grid.GetTile(gridPosition) && tiles.Contains(t) == false));
                }
            }
            
            return tiles.Select(t => t.GridPosition).ToList();
        }
    }
}