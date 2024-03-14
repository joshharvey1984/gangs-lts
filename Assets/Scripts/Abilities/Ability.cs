using System.Collections.Generic;
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
            GridVisualManager.Instance.ResetAllVisuals();
        }

        public virtual void Deselect() {
            Unit.SelectedAbility = null;
        }

        protected virtual void Execute() {
            GameManager.Instance.SquadTurn.ActivatedUnit = true;
            GameManager.Instance.abilityUIPanel.GetComponent<AbilityButtonBar>().DisableAbilityButtons();
        }

        public virtual int ToHit(Tile tile) => 0;

        protected virtual List<ToHitModifier> GetToHitModifiers(Tile fromTile, Tile targetTile) {
            var modifiers = new List<ToHitModifier>();
            if (CheckCoverModifier(fromTile, targetTile) is { } coverModifier) modifiers.Add(coverModifier);
            if (CheckHeightModifier(fromTile, targetTile) is { } heightModifier) modifiers.Add(heightModifier);
            return modifiers;
        }

        private ToHitModifier CheckCoverModifier(Tile fromTile, Tile targetTile) {
            var coverType = GridManager.Instance.CheckTileCover(fromTile, targetTile);
            if (coverType == CoverType.Full) return new ToHitModifier { ModifierType = ToHitModifierType.TargetInCover, Modifier = -20, Description = "Target in full cover" };
            if (coverType == CoverType.Half) return new ToHitModifier { ModifierType = ToHitModifierType.TargetInCover, Modifier = -10, Description = "Target in half cover" };
            return null;
        }
        
        private ToHitModifier CheckHeightModifier(Tile fromTile, Tile targetTile) {
            // try catch block to handle null reference exception
            if (fromTile == null) {
                Debug.LogError("Error: " + fromTile + " " + targetTile);
                return null;
            }
            try {
                return fromTile.GridPosition.Y > targetTile.GridPosition.Y ? 
                    new ToHitModifier { ModifierType = ToHitModifierType.HeightAdvantage, Modifier = 10, Description = "Height advantage" } : null;
            } catch {
                Debug.LogError("Error: " + fromTile + " " + targetTile);
                return null;
            }
        }

        protected void Finish() {
            if (EndTurnOnUse || Unit.ActionPointsRemaining <= 0) {
                GridVisualManager.Instance.ResetAllVisuals();
                GameManager.Instance.SquadTurn.EndUnitTurn();
            }
            
            GameManager.Instance.abilityUIPanel.GetComponent<AbilityButtonBar>().EnableAbilityButtons();
        }
    }
}

public enum TargetingType {
    StandardMove,
    EnemiesInLineOfSight,
}