using System.Collections.Generic;
using Gangs.Core;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Map : Entity {
        public static List<Map> All { get; } = new();
        
        public List<Tile> Tiles { get; private set; }
        public List<Wall> Walls { get; private set; }
        public List<Ladder> Ladders { get; private set; }
        
        public Map(EntityDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(MapDto dto) {
            Tiles = new List<Tile>();
            Walls = new List<Wall>();
            Ladders = new List<Ladder>();
            
            foreach (var dtoTile in dto.tiles) {
                Tiles.Add(new Tile {
                    X = dtoTile.x,
                    Y = dtoTile.y,
                    Z = dtoTile.z
                });
            }
            foreach (var dtoWall in dto.walls) {
                Walls.Add(new Wall {
                    X = dtoWall.x,
                    Y = dtoWall.y,
                    Z = dtoWall.z,
                    CoverType = (CoverType)dtoWall.coverType,
                    LineOfSightBlocker = dtoWall.lineOfSightBlocker
                });
            }
            foreach (var dtoLadder in dto.ladders) {
                Ladders.Add(new Ladder {
                    X = dtoLadder.x,
                    Y = dtoLadder.y,
                    Z = dtoLadder.z,
                    Rotation = dtoLadder.rotation
                });
            }
        }
    }
    
    public class Tile {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
    
    public class Wall {
        public float X { get; set; }
        public int Y { get; set; }
        public float Z { get; set; }
        public CoverType CoverType { get; set; }
        public bool LineOfSightBlocker { get; set; }
    }
    
    public class Ladder {
        public float X { get; set; }
        public int Y { get; set; }
        public float Z { get; set; }
        public int Rotation { get; set; }
    }
}