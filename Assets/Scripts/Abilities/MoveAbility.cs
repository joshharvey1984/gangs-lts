using Gangs.Managers;

namespace Gangs.Abilities {
    public class MoveAbility : BaseAbility {
        public MoveAbility() {
            ButtonText = "Move";
            InputMode = InputMode.Movement;
            TargetingType = TargetingType.StandardMove;
        }
    }
}