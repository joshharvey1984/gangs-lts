using System.Collections.Generic;
using System.Linq;
using Gangs.Calculators;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Abilities {
    public class FireAbility : BaseAbility {
        public FireAbility() {
            ButtonText = "Fire";
            InputMode = InputMode.Attack;
            TargetingType = TargetingType.EnemiesInLineOfSight;
            EndTurnOnUse = true;
        }

        public override void Use(Tile targetTile) {
            var random = new System.Random();
            var toHitRoll = random.Next(1, 101);
            var toHit = ToHit(targetTile);
            if (toHitRoll <= toHit) {
                var targetGridUnit = targetTile.GridUnit;
                var targetUnit = GameManager.Instance.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
                var damage = 10;
                targetUnit.DamageTaken += damage;
                Debug.Log($"Hit! {damage} damage dealt to {targetUnit.Fighter.Name}");
            }
            else {
                Debug.Log("Miss!");
            }
            
            Finish();
        }
        
        public override int ToHit(Tile targetTile) {
            // get unit tile
            var unit = GameManager.Instance.SelectedUnit;
            var targetGridUnit = targetTile.GridUnit;
            var targetUnit = GameManager.Instance.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
            var unitTile = GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            
            // get to hit calculator
            var toHitCalculator = new ToHitCalculator();
            var toHit = toHitCalculator.CalculateToHitChance(unitTile, targetTile, unit, targetUnit, new List<ToHitModifiers>());
            return toHit;
        }
    }
}