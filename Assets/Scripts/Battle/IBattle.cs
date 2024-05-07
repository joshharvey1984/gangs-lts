using System.Collections.Generic;

namespace Gangs.Battle {
    public interface IBattle {
        public BattleBase BattleBase { get; }
        public void StartBattle();
        public List<BattleUnit> GetUnits();
    }
}