using Gangs.Abilities;
using Gangs.Data;
using Gangs.Grid;

namespace Gangs {
    public class Unit {
        public readonly Fighter Fighter;
        public UnitGameObject UnitGameObject;
        public GridUnit GridUnit;
        
        public BaseAbility[] Abilities;
        
        public int ActionPointsRemaining;
        public bool TurnTaken = false;
        
        public int DamageTaken;
        
        public Unit(Fighter fighter) {
            Fighter = fighter;
            ActionPointsRemaining = fighter.GetCurrentAttributeValue(FighterAttribute.ActionPoints);
            Abilities = new BaseAbility[] {
                new MoveAbility(),
                new FireAbility()
            };
        }

        public void SetSelected(bool selected) {
            UnitGameObject.SetSelected(selected);
        }
        
        public void ResetTurn() {
            ActionPointsRemaining = Fighter.GetCurrentAttributeValue(FighterAttribute.ActionPoints);
            TurnTaken = false;
        }
        
        public void SpendActionPoints(int amount) {
            ActionPointsRemaining -= amount;
            if (ActionPointsRemaining < 0) {
                ActionPointsRemaining = 0;
            }
        }
    }
}