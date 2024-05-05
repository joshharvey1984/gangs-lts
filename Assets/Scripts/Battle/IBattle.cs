using Gangs.Battle.Grid;

namespace Gangs.Battle {
    public interface IBattle {
        public Battle Battle { get; }
        public void StartBattle();
    }
}