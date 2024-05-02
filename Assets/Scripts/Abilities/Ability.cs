using System.Collections.Generic;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Calculators;
using Gangs.Core;
using Gangs.Grid;
using Gangs.Managers;
using Gangs.UI;

namespace Gangs.Abilities {
    public abstract class Ability {
        protected readonly Battle.Battle Battle;
        protected readonly BattleUnit BattleUnit;
        protected readonly BattleSquad BattleSquad;
        public string ButtonText;
        protected TargetingType TargetingType;
        protected bool EndTurnOnUse = false;

        protected Ability(BattleUnit battleUnit, BattleSquad battleSquad, Battle.Battle battle) {
            BattleUnit = battleUnit;
            BattleSquad = battleSquad;
            Battle = battle;
        }
        
        public virtual void Select() {
            if (BattleUnit.SelectedAbility != null && BattleUnit.SelectedAbility != this) BattleUnit.SelectedAbility.Deselect();
            BattleUnit.SelectedAbility = this;
        }

        public virtual void Deselect() {
            BattleUnit.SelectedAbility = null;
        }

        public virtual void Execute() {
            BattleSquad.ActivatedUnit = true;
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
            var coverType = Battle.Grid.GetCoverType(fromTile.GridPosition, targetTile.GridPosition);
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
            if (EndTurnOnUse || BattleUnit.ActionPointsRemaining <= 0) {
                GridVisualManager.Instance.ResetAllVisuals();
                BattleSquad.EndUnitTurn();
                return;
            }

            if (!BattleUnit.IsPlayerControlled && BattleUnit.ActionPointsRemaining > 0) {
                EnemyAI.TakeTurn(BattleUnit, Battle);
            }
            
            BattleManager.Instance.abilityUIPanel.GetComponent<AbilityButtonBar>().EnableAbilityButtons();
        }
    }
}

public enum TargetingType {
    StandardMove,
    EnemiesInLineOfSight,
}