using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.Grid;
using Gangs.Calculators;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Abilities {
    public class FireAbility : Ability {
        private List<BattleUnit> _targets;
        private Tile _targetTile;
        
        public event Action<List<Tile>, int> OnDamageDealt;
        
        public FireAbility(BattleUnit battleUnit, BattleGrid battleGrid) : base(battleUnit, battleGrid) {
            ButtonText = "Fire";
            TargetingType = TargetingType.EnemiesInLineOfSight;
            EndTurnOnUse = true;
        }
        
        public override void Select() {
            base.Select();
            
            // _targets = BattleUnit.GetEnemiesInLineOfSight();
            // foreach (var enemy in _targets) {
            //     var enemyTile = GridManager.Instance.Grid.FindGridUnit(enemy.GridUnit);
            //     //GridVisualManager.Instance.ColorTile(enemyTile, Color.red);
            // }
            //
            // if (BattleUnit.IsPlayerControlled) {
            //     InputManager.Instance.OnTileHovered += TileHovered;
            //     InputManager.Instance.OnLeftClickTile += LeftClickTile;
            // }
        }
        
        public override void Deselect() {
            // if (BattleUnit.IsPlayerControlled) {
            //     InputManager.Instance.OnTileHovered -= TileHovered;
            //     InputManager.Instance.OnLeftClickTile -= LeftClickTile;
            // }

            base.Deselect();
        }
        
        // private void TileHovered(Tile tile) {
        //     if (BattleUnit.ActionPointsRemaining <= 0) return;
        //     var targetTiles = _targets.Select(t => GridManager.Instance.Grid.FindGridUnit(t.GridUnit)).ToList();
        //     //targetTiles.ForEach(t => GridVisualManager.Instance.ColorTile(t, Color.red));
        //     if (targetTiles.Contains(tile)) {
        //         //GridVisualManager.Instance.ColorTile(tile, Color.yellow);
        //         var toHit = ToHit(tile);
        //     }
        // }
        
        public void LeftClickTile(Tile tile) {
            _targetTile = tile;
            Execute();
        }

        public override void Execute() {
            base.Execute();
            var random = new System.Random();
            var toHitRoll = random.Next(1, 101);
            var toHit = ToHit(_targetTile);
            if (toHitRoll <= toHit) {
                var targetGridUnit = _targetTile.GridUnit;
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