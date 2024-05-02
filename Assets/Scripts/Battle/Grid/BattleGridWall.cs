using Gangs.Core;
using Gangs.Grid;

namespace Gangs.Battle.Grid {
    public class BattleGridWall {
        public Wall Wall { get; set; }
        public CoverType CoverType { get; set; }
        public bool LineOfSightBlocker { get; set; }
    }
}