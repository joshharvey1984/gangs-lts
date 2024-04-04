using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gangs.Grid {
    public class Grid {
        public Tile[,,] Tiles { get; }
        
        public Grid(int x, int y, int z) {
            Tiles = new Tile[x, y, z];
        }

        public enum TraversalType {
            Move,
            See
        }
        
        public List<Tile> GetValidNeighbours(Tile tile, TraversalType traversalType = TraversalType.Move) {
            var neighbours = GetAllNeighbourPositions(tile.GridPosition);
            var validNeighbours = new List<Tile>();
            
            foreach (var neighborPosition in neighbours) {
                // if there's no tile there, then check if there's a tile below it
                if (GetTile(neighborPosition) == null) {
                    if (neighborPosition.Y == 0) continue;
                    var belowPosition = new GridPosition(neighborPosition.X, neighborPosition.Y - 1, neighborPosition.Z);
                    var neighbourBelow = GetTile(belowPosition);
                    if (neighbourBelow == null) continue;
                    if (PathBlocked(tile.GridPosition, neighbourBelow.GridPosition, traversalType)) continue;
                    validNeighbours.Add(Tiles[neighborPosition.X, neighborPosition.Y - 1, neighborPosition.Z]);
                    continue;
                }
                
                // if there's a tile there, check if there's a prop or wall between the two tiles
                var neighbour = Tiles[neighborPosition.X, neighborPosition.Y, neighborPosition.Z];
                if (PathBlocked(tile.GridPosition, neighbour.GridPosition, traversalType)) continue;
                validNeighbours.Add(Tiles[neighborPosition.X, tile.GridPosition.Y, neighborPosition.Z]);
            }
            
            if (tile.Climbable != null) {
                validNeighbours.Add(tile.Climbable.ConnectedTile(tile));
            }

            return validNeighbours;
        }
        
        public Tile GetTile(GridPosition pos) => !IsPositionWithinGridBounds(pos) ? null : Tiles[pos.X, pos.Y, pos.Z];
        public Tile GetTile(int x, int y, int z) => !IsPositionWithinGridBounds(new GridPosition(x, y, z)) ? null : Tiles[x, y, z];
        public Tile GetTile(float x, float y, float z) => !IsPositionWithinGridBounds(new GridPosition((int)x, (int)y, (int)z)) ? null : Tiles[(int)x, (int)y, (int)z];
        public Tile GetTile(Vector3 position) => !IsPositionWithinGridBounds(new GridPosition((int)position.x, (int)position.y, (int)position.z)) ? null : Tiles[(int)position.x, (int)position.y, (int)position.z];
        public List<Tile> GetTilesByGridPosition(IEnumerable<GridPosition> positions) => positions.Select(GetTile).Where(tile => tile != null).ToList();
        
        public Tile GetClosestTile(Vector3 position) {
            var gridPosition = new GridPosition(position);
            if (IsPositionWithinGridBounds(gridPosition)) return GetTile(gridPosition);
            var closestTile = new GridPosition(Mathf.Clamp(gridPosition.X, 0, Tiles.GetLength(0) - 1), Mathf.Clamp(gridPosition.Y, 0, Tiles.GetLength(1) - 1), Mathf.Clamp(gridPosition.Z, 0, Tiles.GetLength(2) - 1));
            return GetTile(closestTile);
        }
        
        public List<GridPosition> GetGridPositionsInRangeByLevelOffset(GridPosition position, int range, int levelOffset = 0) {
            var positions = new List<GridPosition>();

            for (var i = -range; i <= range; i++) {
                for (var j = -range; j <= range; j++) {
                    var posX = position.X + i;
                    var posZ = position.Z + j;
                    if (!IsPositionWithinGridBounds(new GridPosition(posX, position.Y + levelOffset, posZ))) continue;
                    var gridPosition = new GridPosition(posX, position.Y + levelOffset, posZ);
                    positions.Add(gridPosition);
                }
            }
            
            return positions;
        }

        public bool CanMoveFromTileToTile(Tile from, Tile to) => GetValidNeighbours(from).Contains(to);

        public bool CanSeeFromPositionToPosition(GridPosition from, GridPosition to) => 
            !PathBlocked(from, to, TraversalType.See);

        public bool IsPositionWithinGridBounds(GridPosition position) =>
            position.X >= 0 && position.X < Tiles.GetLength(0) &&
            position.Y >= 0 && position.Y < Tiles.GetLength(1) &&
            position.Z >= 0 && position.Z < Tiles.GetLength(2);
        
        public List<GridPosition> GetAllNeighbourPositions(GridPosition position, bool includeDiagonals = true) =>
            CardinalDirectionExtensions.GetCardinals(includeDiagonals)
                .Select(direction => GridPosition.GridPositionFromDirection(position, direction))
                .Where(IsPositionWithinGridBounds)
                .ToList();

        private bool PathBlocked(GridPosition position, GridPosition neighbour, TraversalType traversalType = TraversalType.Move) {
            var neighbourTile = Tiles[neighbour.X, neighbour.Y, neighbour.Z];
            if (traversalType == TraversalType.Move && neighbourTile.GridUnit != null) return true;
            if (neighbourTile != null && neighbourTile.Prop != null) return true;
            if (IsBlockedByPropOrWall(position, neighbour, traversalType)) return true;
            
            // if there's a prop or wall blocking the diagonals, return true
            var adjacentPosition1 = new GridPosition(position.X, position.Y, neighbour.Z);
            var adjacentPosition2 = new GridPosition(neighbour.X, position.Y, position.Z);
            if (IsBlockedByPropOrWall(position, adjacentPosition1) || IsBlockedByPropOrWall(position, adjacentPosition2)) return true;
            if (IsBlockedByPropOrWall(neighbour, adjacentPosition1) || IsBlockedByPropOrWall(neighbour, adjacentPosition2)) return true;

            return false;
        }
        
        private CardinalDirection GetDirection(GridPosition position, GridPosition adjacentPosition) {
            var dir = adjacentPosition - position;
            return new Direction2D(dir.X, dir.Z).ToCardinalDirection();
        }

        private bool IsBlockedByPropOrWall(GridPosition position, GridPosition adjacentPosition, TraversalType traversalType = TraversalType.Move) {
            if (position.X == adjacentPosition.X && position.Z == adjacentPosition.Z) return false;
            var tile = GetTile(position);
            var adjacentTile = GetTile(adjacentPosition);
            if (adjacentTile != null) {
                if (adjacentTile.Prop != null) return true;
                var dir = GetDirection(position, adjacentPosition);
                if (adjacentTile.Walls.ContainsKey(dir.GetOpposite())) {
                    if (traversalType == TraversalType.See && adjacentTile.Walls[dir.GetOpposite()].LineOfSightBlocker) return true;
                    if (traversalType == TraversalType.Move) return true;
                
                }
            }
            if (tile != null) {
                if (tile.Walls.ContainsKey(GetDirection(position, adjacentPosition))) {
                    if (traversalType == TraversalType.See && tile.Walls[GetDirection(position, adjacentPosition)].LineOfSightBlocker) return true;
                    if (traversalType == TraversalType.Move) return true;
                
                }
            }
            
            return false;
        }

        public Tile FindGridUnit(GridUnit unit) {
            for (var i = 0; i < Tiles.GetLength(0); i++) {
                for (var j = 0; j < Tiles.GetLength(1); j++) {
                    for (var k = 0; k < Tiles.GetLength(2); k++) {
                        if (GetTile(i, j, k) == null) continue;
                        if (Tiles[i, j, k].GridUnit == null) continue;
                        if (Tiles[i, j, k].GridUnit == unit) {
                            return Tiles[i, j, k];
                        }
                    }
                }
            }

            return null;
        }
        
        public void MoveUnit(GridUnit unit, GridPosition to) {
            var from = FindGridUnit(unit);
            if (from.GridPosition == to) return;
            if (Tiles[to.X, to.Y, to.Z].GridUnit != null) return;
            from.GridUnit = null;
            Tiles[to.X, to.Y, to.Z].GridUnit = unit;
        }
        
        public List<Tile> GetAllTiles() {
            var tiles = new List<Tile>();
            for (var i = 0; i < Tiles.GetLength(0); i++) {
                for (var j = 0; j < Tiles.GetLength(1); j++) {
                    for (var k = 0; k < Tiles.GetLength(2); k++) {
                        if (Tiles[i, j, k] != null) tiles.Add(Tiles[i, j, k]);
                    }
                }
            }

            return tiles;
        }
        
        #region Add Methods

        public GridUnit AddUnit(GridPosition gridPosition) {
            var unit = new GridUnit();
            Tiles[gridPosition.X, gridPosition.Y, gridPosition.Z].GridUnit = unit;
            return unit;
        }

        public Wall AddWall(GridPosition tile1, GridPosition tile2, bool losBlocking = true) {
            var wall = new Wall {
                LineOfSightBlocker = losBlocking
            };
            var direction = new Direction2D(tile1 - tile2).ToCardinalDirection();
            if (GetTile(tile1) != null) GetTile(tile1)!.Walls.Add(direction.GetOpposite(), wall);
            if (GetTile(tile2) != null) GetTile(tile2)!.Walls.Add(direction, wall);
            return wall;
        }
        
        public void AddLadder(GridPosition lowerTile, GridPosition upperTile) {
            var ladder = new Climbable();
            Tiles[lowerTile.X, lowerTile.Y, lowerTile.Z].Climbable = ladder;
            Tiles[upperTile.X, upperTile.Y, upperTile.Z].Climbable = ladder;
            
            ladder.LowerTile = Tiles[lowerTile.X, lowerTile.Y, lowerTile.Z];
            ladder.UpperTile = Tiles[upperTile.X, upperTile.Y, upperTile.Z];
        }

        public void AddTile(GridPosition gridPosition) {
            Tiles[gridPosition.X, gridPosition.Y, gridPosition.Z] = new Tile(gridPosition);
        }

        public Prop AddProp(GridPosition gridPosition, bool lineOfSightBlocker = true) {
            var prop = Tiles[gridPosition.X, gridPosition.Y, gridPosition.Z].Prop = new Prop();
            prop.LineOfSightBlocker = lineOfSightBlocker;
            var neighbours = GetAllNeighbourPositions(gridPosition, false);
            foreach (var neighbour in neighbours) {
                var direction = GridPosition.GetCardinalDirection(gridPosition, neighbour);
                GetTile(neighbour)?.NeighbourProps.TryAdd(direction!.Value, prop);
            }
            
            return prop;
        }

        #endregion
    }
}
