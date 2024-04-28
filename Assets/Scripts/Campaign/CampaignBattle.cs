using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Data;
using Gangs.Grid;

namespace Gangs.Campaign {
    public class CampaignBattle {
        public Grid.Grid Grid { get; }
        
        public CampaignBattle(CampaignTerritory territory, CampaignBattleType battleType) {
            var map = territory.Map;
            
            var xSize = map.Tiles.Max(t => t.X) + 1;
            var ySize = map.Tiles.Max(t => t.Y) + 1;
            var zSize = map.Tiles.Max(t => t.Z) + 1;
            Grid = new Grid.Grid(xSize, ySize, zSize);
            
            SetupGrid(map);
            LineOfSight.BuildLineOfSightData(Grid);
            SpawnSquads(territory.Entities);
        }

        private void SpawnSquads(List<ICampaignEntity> territoryEntities) {
            var spawnPositions1 = new List<GridPosition> {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0),
                new(3, 0, 0)
            };
            
            var spawnPositions2 = new List<GridPosition> {
                new(Grid.Tiles.GetLength(0) - 1, 0, Grid.Tiles.GetLength(2) - 1),
                new(Grid.Tiles.GetLength(0) - 2, 0, Grid.Tiles.GetLength(2) - 1),
                new(Grid.Tiles.GetLength(0) - 3, 0, Grid.Tiles.GetLength(2) - 1),
                new(Grid.Tiles.GetLength(0) - 4, 0, Grid.Tiles.GetLength(2) - 1)
            };

            var pos = 0;
            
            territoryEntities.ForEach(entity => {
                if (entity is CampaignSquad squad) {
                    var spawnPositions = pos == 0 ? spawnPositions1 : spawnPositions2;
                    var spawnPosition = spawnPositions.First();
                    spawnPositions.Remove(spawnPosition);
                    var gridUnit = Grid.AddUnit(new GridPosition(spawnPosition.X, spawnPosition.Y, spawnPosition.Z));
                }
                else if (entity is CampaignMob mob) {
                    var spawnPositions = pos == 0 ? spawnPositions1 : spawnPositions2;
                    var spawnPosition = spawnPositions.First();
                    spawnPositions.Remove(spawnPosition);
                    var gridUnit = Grid.AddUnit(new GridPosition(spawnPosition.X, spawnPosition.Y, spawnPosition.Z));
                }
                
                pos++;
            });
        }


        private void SetupGrid(Map map) {
            foreach (var tile in map.Tiles) {
                Grid.AddTile(new GridPosition(tile.X, tile.Y, tile.Z));
            }
            
            foreach (var wall in map.Walls) {
                if (Math.Abs(wall.Z % 1 - 0.5f) < 0.01) {
                    var tile1 = new GridPosition(wall.X, wall.Y, wall.Z + 0.5f);
                    var tile2 = new GridPosition(wall.X, wall.Y, wall.Z - 0.5f);
                    Grid.AddWall(tile1, tile2);
                }
                else {
                    var tile1 = new GridPosition(wall.X - 0.5f, wall.Y, wall.Z);
                    var tile2 = new GridPosition(wall.X - 0.5f, wall.Y, wall.Z);
                    Grid.AddWall(tile1, tile2);
                }
            }

            foreach (var ladder in map.Ladders) {
                var upperLadderPosition = ladder switch {
                    { Rotation: 0 } => new GridPosition(ladder.X, ladder.Y + 1, ladder.Z + 1),
                    { Rotation: 180 } => new GridPosition(ladder.X, ladder.Y + 1, ladder.Z - 1),
                    { Rotation: 90 } => new GridPosition(ladder.X + 1, ladder.Y + 1, ladder.Z),
                    { Rotation: 270 } => new GridPosition(ladder.X - 1, ladder.Y + 1, ladder.Z),
                    _ => new GridPosition()
                };

                Grid.AddLadder(new GridPosition(ladder.X, ladder.Y, ladder.Z), upperLadderPosition);
            }
        }
    }
    
    public enum CampaignBattleType {
        Auto,
        Manual
    }
}