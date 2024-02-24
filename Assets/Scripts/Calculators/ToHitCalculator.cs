using System;
using System.Collections.Generic;
using Gangs.Data;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Calculators {
    public class ToHitCalculator {
        private const float DecayRate = 0.1f;
        
        public int CalculateToHitChance(Tile fromTile, Tile targetTile, Unit firingUnit, Unit targetUnit, List<ToHitModifiers> modifiers) {
            var range = GridPosition.Distance(fromTile.GridPosition, targetTile.GridPosition);
            var toHitChance = (int)(100 * Math.Exp(-DecayRate * range));
            var firingUnitSkill = firingUnit.Fighter.GetCurrentAttributeValue(FighterAttribute.Aim);
            toHitChance += (firingUnitSkill * 5);
            modifiers.ForEach(modifier => toHitChance += modifier.Modifier);
            
            toHitChance = Mathf.Clamp(toHitChance, 0, 100);
            return toHitChance;
        }
    }

    public class ToHitModifiers {
        public int Modifier;
        public string Reason;
    }
}