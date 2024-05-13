using System;
using System.Collections.Generic;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Calculators;
using Gangs.Core;
using Gangs.Grid;

namespace Gangs.Abilities {
    public abstract class Ability {
        protected readonly BattleUnit BattleUnit;
        protected readonly BattleGrid BattleGrid;
        
        public string ButtonText;
        public bool EndTurnOnUse = false;
        public TargetingType TargetingType { get; protected set; }
        
        public List<TargetTiles> TargetTiles { get; private set; }
        public Tile TargetTile { get; set; }
        
        public event Action<Ability> OnAbilitySelected;
        public event Action<Tile> OnAbilityExecuted;
        public event Action OnAbilityFinished;
        
        protected Ability(BattleUnit battleUnit, BattleGrid battleGrid, Action<Tile> onAbilityExecuted = null) {
            BattleUnit = battleUnit;
            BattleGrid = battleGrid;
            OnAbilityExecuted = onAbilityExecuted;
        }
        
        public virtual void Select() {
            if (BattleUnit.SelectedAbility != null && BattleUnit.SelectedAbility != this) BattleUnit.SelectedAbility.Deselect();
            BattleUnit.SelectedAbility = this;
            TargetTiles = TargetingType.GetTargetingTiles(BattleUnit.GridUnit.GetTile(), BattleUnit, BattleGrid);
            OnAbilitySelected?.Invoke(this);
        }

        public virtual void Deselect() {
            BattleUnit.SelectedAbility = null;
            TargetTile = null;
            TargetTiles = null;
        }
        
        public void SetTarget(Tile tile) {
            if (tile == null) return;
            if (TargetTiles == null) return;
            if (!TargetTiles.Exists(t => t.Tiles.Contains(tile))) return;
            TargetTile = tile;
            Execute();
        }

        public virtual void Execute() {
            var apCost = TargetTiles.Find(t => t.Tiles.Contains(TargetTile)).Cost;
            BattleUnit.SpendActionPoints(apCost);
            BattleUnit.OnAbilityCompleted += Finish;
            OnAbilityExecuted?.Invoke(TargetTile);
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
            var coverType = BattleGrid.GetCoverType(fromTile.GridPosition, targetTile.GridPosition);
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
            OnAbilityFinished?.Invoke();
        }
    }
}