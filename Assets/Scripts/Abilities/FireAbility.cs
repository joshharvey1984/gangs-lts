using System.Collections.Generic;
using System.Linq;
using Gangs.Calculators;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Abilities {
    public class FireAbility : Ability {
        private List<Unit> _targets;
        public FireAbility(Unit unit) : base(unit) {
            ButtonText = "Fire";
            TargetingType = TargetingType.EnemiesInLineOfSight;
            EndTurnOnUse = true;
        }
        
        public override void Select() {
            base.Select();
            
            _targets = Unit.GetEnemiesInLineOfSight();
            foreach (var enemy in _targets) {
                var enemyTile = GridManager.Instance.Grid.FindGridUnit(enemy.GridUnit);
                GridVisualManager.Instance.ColorTile(enemyTile, Color.red);
            }
            
            InputManager.Instance.OnTileHovered += TileHovered;
            InputManager.Instance.OnLeftClickTile += LeftClickTile;
        }
        
        public override void Deselect() {
            InputManager.Instance.OnTileHovered -= TileHovered;
            InputManager.Instance.OnLeftClickTile -= LeftClickTile;
            base.Deselect();
        }
        
        private void TileHovered(Tile tile) {
            if (Unit.ActionPointsRemaining <= 0) return;
            var targetTiles = _targets.Select(t => GridManager.Instance.Grid.FindGridUnit(t.GridUnit)).ToList();
            targetTiles.ForEach(t => GridVisualManager.Instance.ColorTile(t, Color.red));
            if (targetTiles.Contains(tile)) {
                GridVisualManager.Instance.ColorTile(tile, Color.yellow);
                var toHit = ToHit(tile);
            }
        }
        
        private void LeftClickTile(Tile tile) {
            Execute();
        }

        protected override void Execute() {
            base.Execute();
            var targetTile = InputManager.Instance.HoverTile;
            var random = new System.Random();
            var toHitRoll = random.Next(1, 101);
            var toHit = ToHit(targetTile);
            if (toHitRoll <= toHit) {
                var targetGridUnit = targetTile.GridUnit;
                var targetUnit = GameManager.Instance.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
                var damage = 10;
                targetUnit!.DamageTaken += damage;
                Debug.Log($"Hit! {damage} damage dealt to {targetUnit.Fighter.Name}");
            }
            else {
                Debug.Log("Miss!");
            }
            
            Finish();
        }
        
        public override int ToHit(Tile targetTile) {
            // get unit tile
            var unit = GameManager.Instance.SquadTurn.SelectedUnit;
            var targetGridUnit = targetTile.GridUnit;
            var targetUnit = GameManager.Instance.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
            var unitTile = GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            
            
            // get to hit calculator
            var toHitCalculator = new ToHitCalculator();
            var modifiers = GetToHitModifiers(unitTile, targetTile);
            var toHit = toHitCalculator.CalculateToHitChance(unitTile, targetTile, unit, targetUnit, modifiers);
            return toHit;
        }
    }
}