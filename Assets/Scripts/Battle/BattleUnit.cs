using System;
using Gangs.Abilities;
using Gangs.Battle.Grid;
using Gangs.Campaign;
using Gangs.Core;
using Gangs.Grid;

namespace Gangs.Battle {
    public class BattleUnit {
        public readonly CampaignUnit Unit;
        public GridUnit GridUnit;
        
        public readonly Ability[] Abilities;
        
        public Ability SelectedAbility;
        
        public Status Status = Status.Active;
        
        public int ActionPointsRemaining;
        public bool TurnTaken = false;
        
        public int DamageTaken;

        public bool IsPlayerControlled = false;
        
        public event Action<BattleUnit> OnDeselected;
        public event Action<BattleUnit> OnSelected; 
        public event Action<BattleUnit> OnUnitEliminated;
        
        public BattleUnit(CampaignUnit unit, BattleGrid battleGrid) {
            Unit = unit;
            ActionPointsRemaining = GetAttributeValue(UnitAttributeType.ActionPoints);
            Abilities = new Ability[] {
                new MoveAbility(this, battleGrid),
                new FireAbility(this, battleGrid)
            };
        }

        public int GetAttributeValue(UnitAttributeType attributeType) => Unit.GetAttribute(attributeType).GetValue();

        public void SetSelected(bool selected) {
            (selected ? OnSelected : OnDeselected)?.Invoke(this);
            
            if (selected == false) SelectedAbility?.Deselect(); 
            else Abilities[0]?.Select();
            
            //if (TurnTaken) UnitGameObject.SetSelected(SelectionCircle.State.Unavailable);
            //else UnitGameObject.SetSelected(selected ? SelectionCircle.State.Selected : SelectionCircle.State.Available);
        }

        public void ResetTurn() {
            ActionPointsRemaining = GetAttributeValue(UnitAttributeType.ActionPoints);
            TurnTaken = false;
        }
        
        public void SpendActionPoints(int amount) {
            ActionPointsRemaining -= amount;
            if (ActionPointsRemaining < 0) {
                ActionPointsRemaining = 0;
            }
        }
        
        public void Damage(int amount) {
            DamageTaken += amount;
            if (DamageTaken >= GetCurrentHitPoints()) {
                Eliminate();
            }
        }
        
        private void Eliminate() {
            Status = Status.Eliminated;
            //GridManager.Instance.RemoveGridUnit(GridUnit);
            OnUnitEliminated?.Invoke(this);
        }
        
        public int GetCurrentHitPoints() => GetAttributeValue(UnitAttributeType.HitPoints) - DamageTaken;

        public void SetSelectedAbility(Ability ability) {
            ability.Select();
        }
    }
    
    public enum Status {
        Active,
        Knocked,
        Eliminated
    }
}