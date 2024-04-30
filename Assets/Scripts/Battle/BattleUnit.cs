using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Campaign;
using Gangs.Core;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.Managers;
using Gangs.UI;

namespace Gangs.Battle {
    public class BattleUnit {
        public readonly CampaignUnit Unit;
        public UnitGameObject UnitGameObject;
        public GridUnit GridUnit;
        
        public Ability[] Abilities;
        
        public Ability SelectedAbility;
        
        public Status Status = Status.Active;
        
        public int ActionPointsRemaining;
        public bool TurnTaken = false;
        
        public int DamageTaken;

        public bool IsPlayerControlled;
        
        public event Action<BattleUnit> OnDeselected;
        public event Action<BattleUnit> OnSelected; 
        
        public BattleUnit(CampaignUnit unit) {
            Unit = unit;
            ActionPointsRemaining = GetAttributeValue(UnitAttributeType.ActionPoints);
            Abilities = new Ability[] {
                new MoveAbility(this),
                new FireAbility(this)
            };
        }

        public int GetAttributeValue(UnitAttributeType attributeType) => Unit.GetAttribute(attributeType).GetValue();

        public void SetSelected(bool selected) {
            (selected ? OnSelected : OnDeselected)?.Invoke(this);
            
            if (selected == false) SelectedAbility?.Deselect(); 
            else Abilities[0]?.Select();
            
            if (TurnTaken) UnitGameObject.SetSelected(SelectionCircle.State.Unavailable);
            else UnitGameObject.SetSelected(selected ? SelectionCircle.State.Selected : SelectionCircle.State.Available);
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
            GridManager.Instance.RemoveGridUnit(GridUnit);
            UnitGameObject.Eliminate();
            BattleManager.Instance.CheckForEndGame();
        }
        
        public int GetCurrentHitPoints() => GetAttributeValue(UnitAttributeType.HitPoints) - DamageTaken;

        public void SetSelectedAbility(Ability ability) {
            ability.Select();
        }
        
        public List<BattleUnit> GetEnemiesInLineOfSight() {
            var units = new List<BattleUnit>();
            var lineOfSight = BattleManager.Instance.GetSoldierTile(this).LineOfSightGridPositions;
            var activeEnemySquads = BattleManager.Instance.Squads.Where(s => s != BattleManager.Instance.BattleSquadTurn).ToList();
            var activeEnemyUnits = activeEnemySquads.SelectMany(s => s.Units).Where(u => u.Status != Status.Eliminated).ToList();
            foreach (var activeEnemy in activeEnemyUnits) {
                var enemyTile = BattleManager.Instance.GetSoldierTile(activeEnemy);
                if (lineOfSight.Contains(enemyTile.GridPosition)) units.Add(activeEnemy);
            }
            return units;
        }
    }
    
    public enum Status {
        Active,
        Knocked,
        Eliminated
    }
}