using System.Collections.Generic;
using Gangs.AI;
using Gangs.Calculators;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.Managers;
using Gangs.UI;
using UnityEngine;

namespace Gangs.Abilities {
    public abstract class Ability {
        protected readonly Unit Unit;
        public string ButtonText;
        protected TargetingType TargetingType;
        protected bool EndTurnOnUse = false;

        protected Ability(Unit unit) {
            Unit = unit;
        }
        
        public virtual void Select() {
            if (Unit.SelectedAbility != null && Unit.SelectedAbility != this) Unit.SelectedAbility.Deselect();
            Unit.SelectedAbility = this;
        }

        public virtual void Deselect() {
            Unit.SelectedAbility = null;
        }

        public virtual void Execute() {
            BattleManager.Instance.SquadTurn.ActivatedUnit = true;
            BattleManager.Instance.abilityUIPanel.GetComponent<AbilityButtonBar>().DisableAbilityButtons();
        }

        public virtual int ToHit(Tile tile) => 0;

        public virtual List<ToHitModifier> GetToHitModifiers(Tile fromTile, Tile targetTile, int apRemaining = 0) {
            var modifiers = new List<ToHitModifier>();
            if (CheckCoverModifier(fromTile, targetTile) is { } coverModifier) modifiers.Add(coverModifier);
            if (CheckHeightModifier(fromTile, targetTile) is { } heightModifier) modifiers.Add(heightModifier);
            if (RemainingActionPointsModifier(apRemaining) is { } apModifier) modifiers.Add(apModifier);
            // if (modifiers.Count > 0) {
            //     var modifierString = string.Join(", ", modifiers.Select(m => m.Description + " " + m.Modifier));
            //     Debug.Log($"To hit modifiers for {fromTile}: {modifierString}");
            // }
            return modifiers;
        }

        private ToHitModifier CheckCoverModifier(Tile fromTile, Tile targetTile) {
            var coverType = GridManager.Instance.CheckTileCover(fromTile, targetTile);
            if (coverType == CoverType.Full) return new ToHitModifier { ModifierType = ToHitModifierType.TargetInCover, Modifier = -20, Description = "Target in full cover" };
            if (coverType == CoverType.Half) return new ToHitModifier { ModifierType = ToHitModifierType.TargetInCover, Modifier = -10, Description = "Target in half cover" };
            return null;
        }
        
        private ToHitModifier RemainingActionPointsModifier(int remainingActionPoints) {
            if (remainingActionPoints <= 1) return null;
            return new ToHitModifier { ModifierType = ToHitModifierType.RemainingActionPoints, Modifier = (remainingActionPoints - 1) * 5, Description = "Remaining action points" };
        }
        
        private ToHitModifier CheckHeightModifier(Tile fromTile, Tile targetTile) {
            return fromTile.GridPosition.Y > targetTile.GridPosition.Y ? 
                new ToHitModifier { ModifierType = ToHitModifierType.HeightAdvantage, Modifier = 10, Description = "Height advantage" } : null;
        }

        protected void Finish() {
            if (EndTurnOnUse || Unit.ActionPointsRemaining <= 0) {
                GridVisualManager.Instance.ResetAllVisuals();
                BattleManager.Instance.SquadTurn.EndUnitTurn();
                return;
            }

            if (!Unit.IsPlayerControlled && Unit.ActionPointsRemaining > 0) {
                EnemyAI.TakeTurn(Unit);
            }
            
            BattleManager.Instance.abilityUIPanel.GetComponent<AbilityButtonBar>().EnableAbilityButtons();
        }
    }
}

public enum TargetingType {
    StandardMove,
    EnemiesInLineOfSight,
}