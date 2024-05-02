using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Calculators;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.Abilities {
    public class FireAbility : Ability {
        private List<BattleUnit> _targets;
        private Tile _targetTile;
        public FireAbility(BattleUnit battleUnit, BattleSquad battleSquad, Battle.Battle battle) : base(battleUnit, battleSquad, battle) {
            ButtonText = "Fire";
            TargetingType = TargetingType.EnemiesInLineOfSight;
            EndTurnOnUse = true;
        }
        
        public override void Select() {
            base.Select();
            
            //_targets = BattleUnit.GetEnemiesInLineOfSight();
            foreach (var enemy in _targets) {
                var enemyTile = GridManager.Instance.Grid.FindGridUnit(enemy.GridUnit);
                //GridVisualManager.Instance.ColorTile(enemyTile, Color.red);
            }

            if (BattleUnit.IsPlayerControlled) {
                InputManager.Instance.OnTileHovered += TileHovered;
                InputManager.Instance.OnLeftClickTile += LeftClickTile;
            }
        }
        
        public override void Deselect() {
            if (BattleUnit.IsPlayerControlled) {
                InputManager.Instance.OnTileHovered -= TileHovered;
                InputManager.Instance.OnLeftClickTile -= LeftClickTile;
            }

            base.Deselect();
        }
        
        private void TileHovered(Tile tile) {
            if (BattleUnit.ActionPointsRemaining <= 0) return;
            var targetTiles = _targets.Select(t => GridManager.Instance.Grid.FindGridUnit(t.GridUnit)).ToList();
            //targetTiles.ForEach(t => GridVisualManager.Instance.ColorTile(t, Color.red));
            if (targetTiles.Contains(tile)) {
                //GridVisualManager.Instance.ColorTile(tile, Color.yellow);
                var toHit = ToHit(tile);
            }
        }
        
        public void LeftClickTile(Tile tile) {
            _targetTile = tile;
            Execute();
        }

        public override void Execute() {
            try {
                base.Execute();
                if (_targetTile == null) _targetTile = InputManager.Instance.HoverTile;
                var random = new System.Random();
                var toHitRoll = random.Next(1, 101);
                var toHit = ToHit(_targetTile);
                if (toHitRoll <= toHit) {
                    var targetGridUnit = _targetTile.GridUnit;
                    var targetUnit = Battle.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
                    var damage = 10;
                    Debug.Log($"Hit! {damage} damage dealt to {targetUnit.Unit.Name}");
                    Debug.Log($"{targetUnit.Unit.Name} has {targetUnit.GetCurrentHitPoints()} hit points remaining");
                    targetUnit!.Damage(damage);
                }
                else {
                    Debug.Log("Miss!");
                }
            }
            catch (Battle.Battle.EndGameException) {
                Deselect();
                Battle.EndGame();
                return;
            }
            
            Finish();
        }
        
        public override int ToHit(Tile targetTile) {
            // get unit tile
            var unit = Battle.SelectedBattleUnit;
            var targetGridUnit = targetTile.GridUnit;
            var targetUnit = Battle.Squads.SelectMany(squad => squad.Units).FirstOrDefault(u => u.GridUnit == targetGridUnit);
            var unitTile = GridManager.Instance.Grid.FindGridUnit(unit.GridUnit);
            
            
            // get to hit calculator
            var toHitCalculator = new ToHitCalculator();
            var modifiers = GetToHitModifiers(unitTile, targetTile, BattleUnit.ActionPointsRemaining);
            var toHit = toHitCalculator.CalculateToHitChance(unitTile, targetTile, unit, targetUnit, modifiers);
            return toHit;
        }
    }
}