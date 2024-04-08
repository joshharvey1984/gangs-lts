using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Data;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.Managers;
using Gangs.UI;

namespace Gangs {
    public class Unit {
        public readonly Fighter Fighter;
        public UnitGameObject UnitGameObject;
        public GridUnit GridUnit;
        
        public Ability[] Abilities;
        
        public Ability SelectedAbility;
        
        public Status Status = Status.Active;
        
        public int ActionPointsRemaining;
        public bool TurnTaken = false;
        
        public int DamageTaken;

        public bool IsPlayerControlled;
        
        public Dictionary<Unit, Tile> EnemyLastSeen = new();
        
        public event Action<Unit> OnDeselected;
        public event Action<Unit> OnSelected; 
        
        public Unit(Fighter fighter) {
            Fighter = fighter;
            ActionPointsRemaining = fighter.GetCurrentAttributeValue(FighterAttribute.ActionPoints);
            Abilities = new Ability[] {
                new MoveAbility(this),
                new FireAbility(this)
            };
        }
        
        public int GetAttribute(FighterAttribute attribute) => Fighter.GetCurrentAttributeValue(attribute);

        public void SetSelected(bool selected) {
            (selected ? OnSelected : OnDeselected)?.Invoke(this);
            
            if (selected == false) SelectedAbility?.Deselect(); 
            else Abilities[0]?.Select();
            
            if (TurnTaken) UnitGameObject.SetSelected(SelectionCircle.State.Unavailable);
            else UnitGameObject.SetSelected(selected ? SelectionCircle.State.Selected : SelectionCircle.State.Available);
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
        
        public void Damage(int amount) {
            DamageTaken += amount;
            if (DamageTaken >= Fighter.GetCurrentAttributeValue(FighterAttribute.HitPoints)) {
                Eliminate();
            }
        }
        
        private void Eliminate() {
            Status = Status.Eliminated;
            GridManager.Instance.RemoveGridUnit(GridUnit);
            UnitGameObject.Eliminate();
            GameManager.Instance.CheckForEndGame();
        }
        
        public int GetCurrentHitPoints() => Fighter.GetCurrentAttributeValue(FighterAttribute.HitPoints) - DamageTaken;

        public void SetSelectedAbility(Ability ability) {
            ability.Select();
        }
        
        public List<Unit> GetEnemiesInLineOfSight() {
            var units = new List<Unit>();
            var lineOfSight = GameManager.Instance.GetSoldierTile(this).LineOfSightGridPositions;
            var activeEnemySquads = GameManager.Instance.Squads.Where(s => s != GameManager.Instance.SquadTurn).ToList();
            var activeEnemyUnits = activeEnemySquads.SelectMany(s => s.Units).Where(u => u.Status != Status.Eliminated).ToList();
            foreach (var activeEnemy in activeEnemyUnits) {
                var enemyTile = GameManager.Instance.GetSoldierTile(activeEnemy);
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