using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Core;
using Gangs.Data;
using Gangs.Grid;
using Tile = Gangs.Grid.Tile;
using Wall = Gangs.Data.Wall;

namespace Gangs.Battle.Grid {
    public class BattleGrid {
        public Gangs.Grid.Grid Grid { get; private set; }

        private List<BattleGridWall> Walls { get; } = new();
        
        public event Func<GridUnit, BattleUnit> OnGetUnit;

        public BattleGrid(Map map) {
            CreateGrid(map);
            Pathfinder.Initialize(Grid);
        }
        
        public void MoveUnit(BattleUnit unit, Tile tile) {
            Grid.MoveUnit(unit.GridUnit, tile.GridPosition);
            // TODO: Update last seen for enemy units
        }

        public IEnumerable<BattleUnit> GetUnitsInSightOfTile(Tile tile, int range) {
            var units = new List<BattleUnit>();
            foreach (var gridPosition in tile.LineOfSightGridPositions) {
                var gridTile = Grid.GetTile(gridPosition);
                if (gridTile == null) continue;
                if (Grid.GetDistance(tile, gridTile) <= range)
                    units.AddRange(gridTile.GridUnit != null ? new[] { GetUnit(gridTile.GridUnit) } : 
                        Array.Empty<BattleUnit>());
            }

            return units;
        }

        public BattleUnit GetUnit(GridUnit gridUnit) {
            var tile = Grid.GetTileByGridUnit(gridUnit);
            return tile != null ? OnGetUnit?.Invoke(gridUnit) : null;
        }

        public CoverType GetCoverType(GridPosition gridPosition, GridPosition targetGridPosition) {
            var dir = GridPosition.GetCardinalDirection(gridPosition, targetGridPosition);
            return dir != null ? GetCoverType(gridPosition, dir.Value) : CoverType.None;
        }

        private CoverType GetCoverType(GridPosition gridPosition, CardinalDirection cardinalDirection) {
            var gridWall = Grid.GetWall(gridPosition, cardinalDirection);
            return Walls.FirstOrDefault(w => w.Wall == gridWall)?.CoverType ?? CoverType.None;
        }

        private void CreateGrid(Map map) {
            var xSize = map.Tiles.Max(t => t.X) + 1;
            var ySize = map.Tiles.Max(t => t.Y) + 1;
            var zSize = map.Tiles.Max(t => t.Z) + 1;
            Grid = new Gangs.Grid.Grid(xSize, ySize, zSize);
            
            SetupGrid(Grid, map);
            LineOfSight.BuildLineOfSightData(Grid);
        }
        
        private void SetupGrid(Gangs.Grid.Grid grid, Map map) {
            foreach (var mapTile in map.Tiles) {
                grid.AddTile(new GridPosition(mapTile.X, mapTile.Y, mapTile.Z));
            }
            
            foreach (var mapWall in map.Walls) {
                if (Math.Abs(mapWall.Z % 1 - 0.5f) < 0.01) {
                    var tile1 = new GridPosition(mapWall.X, mapWall.Y, mapWall.Z + 0.5f);
                    var tile2 = new GridPosition(mapWall.X, mapWall.Y, mapWall.Z - 0.5f);
                    AddWall(tile1, tile2, mapWall);
                }
                else {
                    var tile1 = new GridPosition(mapWall.X - 0.5f, mapWall.Y, mapWall.Z);
                    var tile2 = new GridPosition(mapWall.X - 0.5f, mapWall.Y, mapWall.Z);
                    AddWall(tile1, tile2, mapWall);
                }
            }

            foreach (var mapLadder in map.Ladders) {
                var upperLadderPosition = mapLadder switch {
                    { Rotation: 0 } => new GridPosition(mapLadder.X, mapLadder.Y + 1, mapLadder.Z + 1),
                    { Rotation: 180 } => new GridPosition(mapLadder.X, mapLadder.Y + 1, mapLadder.Z - 1),
                    { Rotation: 90 } => new GridPosition(mapLadder.X + 1, mapLadder.Y + 1, mapLadder.Z),
                    { Rotation: 270 } => new GridPosition(mapLadder.X - 1, mapLadder.Y + 1, mapLadder.Z),
                    _ => new GridPosition()
                };

                grid.AddLadder(new GridPosition(mapLadder.X, mapLadder.Y, mapLadder.Z), upperLadderPosition);
            }
        }

        private void AddWall(GridPosition tile1, GridPosition tile2, Wall wall) {
            var gridWall = Grid.AddWall(tile1, tile2);
            Walls.Add(new BattleGridWall { Wall = gridWall, CoverType = wall.CoverType, LineOfSightBlocker = wall.LineOfSightBlocker });
        }
    }
}