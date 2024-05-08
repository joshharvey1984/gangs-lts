using System;
using System.Collections.Generic;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
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
        
        public List<MoveWaypoint> MoveWaypoints = new();
        
        public event Action OnDeselected;
        public event Action OnSelected;
        public event Action<BattleUnit> OnUnitEliminated;
        public event Action OnAbilityCompleted;
        public event Action<BattleUnit, MoveWaypoint> OnMoveUnitTile;
        
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
            (selected ? OnSelected : OnDeselected)?.Invoke();
            
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
            OnUnitEliminated?.Invoke(this);
        }
        
        public int GetCurrentHitPoints() => GetAttributeValue(UnitAttributeType.HitPoints) - DamageTaken;

        public void SetSelectedAbility(Ability ability) {
            ability.Select();
        }

        public void SetMoveWaypoints(List<MoveWaypoint> moveWaypoints) {
            MoveWaypoints = moveWaypoints;
            MoveWaypoints.RemoveAt(0);
            MoveNextWaypointTile();
        }
        
        public void MoveNextWaypointTile() {
            if (MoveWaypoints.Count == 0) {
                OnAbilityCompleted?.Invoke();
                return;
            }

            var nextWaypoint = new MoveWaypoint {
                DirectPathTiles = new List<Tile>(MoveWaypoints[0].DirectPathTiles),
                Tiles = new List<Tile>(MoveWaypoints[0].Tiles),
            };
            
            MoveWaypoints.RemoveAt(0);
            OnMoveUnitTile?.Invoke(this, nextWaypoint);
        }
    }
    
    public enum Status {
        Active,
        Knocked,
        Eliminated
    }
}