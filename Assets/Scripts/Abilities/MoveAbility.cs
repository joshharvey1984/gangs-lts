using System;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Abilities {
    public class MoveAbility : Ability {
        public MoveAbility(BattleUnit battleUnit, BattleGrid battleGrid, BattleBase battle, Action<Tile> onAbilityExecuted) : base(battleUnit, battleGrid, battle, onAbilityExecuted) {
            ButtonText = "Move";
            TargetingType = TargetingType.StandardMove;
        }
    }
}