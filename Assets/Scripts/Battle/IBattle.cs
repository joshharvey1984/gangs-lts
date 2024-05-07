using System;
using System.Collections.Generic;
using Gangs.Campaign;
using Gangs.Grid;

namespace Gangs.Battle {
    public interface IBattle {
        public BattleBase BattleBase { get; }
        public void StartBattle();
        public List<BattleUnit> GetUnits();
        void MoveUnit(BattleUnit unit, Tile tile);
        event Action<CampaignSquad> OnEndBattle;
    }
}