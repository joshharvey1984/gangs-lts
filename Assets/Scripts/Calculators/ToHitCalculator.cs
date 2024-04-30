using System;
using System.Collections.Generic;
using Gangs.Battle;
using Gangs.Core;
using Gangs.Data;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Calculators {
    public class ToHitCalculator {
        private const float DecayRate = 0.1f;
        
        public int CalculateToHitChance(Tile fromTile, Tile targetTile, BattleUnit firingBattleUnit, BattleUnit targetBattleUnit, List<ToHitModifier> modifiers) {
            var range = GridPosition.Distance(fromTile.GridPosition, targetTile.GridPosition);
            var toHitChance = (int)(100 * Math.Exp(-DecayRate * range));
            var firingUnitSkill = firingBattleUnit.GetAttributeValue(UnitAttributeType.Aim);
            toHitChance += (firingUnitSkill * 5);
            modifiers.ForEach(modifier => toHitChance += modifier.Modifier);
            
            toHitChance = Mathf.Clamp(toHitChance, 0, 100);
            //Debug.Log($"To hit chance: {toHitChance}%");
            //modifiers.ForEach(modifier => Debug.Log($"To hit modifier: {modifier.Description}: {modifier.Modifier}%"));
            return toHitChance;
        }
    }

    public class ToHitModifier {
        public ToHitModifierType ModifierType { get; set; }
        public int Modifier { get; set; }
        public string Description { get; set; }
    }
    
    public enum ToHitModifierType
    {
        TargetInCover,
        HeightAdvantage,
        RemainingActionPoints
    }
}