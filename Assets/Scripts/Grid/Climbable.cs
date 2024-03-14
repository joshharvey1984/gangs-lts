namespace Gangs.Grid {
    public class Climbable {
        public Tile UpperTile { get; set; }
        public Tile LowerTile { get; set; }
        
        public Tile ConnectedTile(Tile tile) => tile == UpperTile ? LowerTile : UpperTile;
    }
}