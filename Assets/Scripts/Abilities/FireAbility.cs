using System;
using System.Collections.Generic;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Calculators;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Abilities {
    public class FireAbility : Ability {
        private List<BattleUnit> _targets;
        
        public event Action<List<Tile>, int> OnDamageDealt;
        
        public FireAbility(BattleUnit battleUnit, BattleGrid battleGrid) : base(battleUnit, battleGrid) {
            ButtonText = "Fire";
            TargetingType = TargetingType.EnemiesInLineOfSight;
            EndTurnOnUse = true;
        }

        public override void Execute() {
            base.Execute();
            var random = new System.Random();
            var toHitRoll = random.Next(1, 101);
            var toHit = ToHit(TargetTile);
            if (toHitRoll <= toHit) {
                var targetGridUnit = TargetTile.GridUnit;
                var damage = 10;
                Debug.Log($"Hit! {BattleUnit.Unit.Name} Dealt {damage} damage to {BattleGrid.GetUnit(targetGridUnit).Unit.Name}");
                Debug.Log($"{BattleGrid.GetUnit(targetGridUnit).Unit.Name} has {BattleGrid.GetUnit(targetGridUnit).GetCurrentHitPoints() - damage} hit points remaining");
                BattleGrid.GetUnit(targetGridUnit).Damage(damage);
            }
            else {
                Debug.Log("Miss!");
            }
            
            Finish();
        }
        
        public override int ToHit(Tile targetTile) {
            // get unit tile
            var unit = BattleUnit;
            var targetGridUnit = targetTile.GridUnit;
            var targetUnit = BattleGrid.GetUnit(targetGridUnit);
            var unitTile = BattleUnit.GridUnit.GetTile();
            
            
            // get to hit calculator
            var toHitCalculator = new ToHitCalculator();
            var modifiers = GetToHitModifiers(unitTile, targetTile, BattleUnit.ActionPointsRemaining);
            var toHit = toHitCalculator.CalculateToHitChance(unitTile, targetTile, unit, targetUnit, modifiers);
            return toHit;
        }
    }
}