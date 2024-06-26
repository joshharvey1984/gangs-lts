﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Grid;

namespace Gangs.Battle {
    public class BattleSquad {
        public readonly List<BattleUnit> Units = new();
        public List<BattleUnit> ActiveUnits => Units.Where(u => u.UnitStatus == UnitStatus.Active).ToList();
        
        public BattleUnit SelectedUnit { get; set; }
        public bool ActivatedUnit;
        
        
        public Dictionary<BattleUnit, Tile> EnemyLastSeen = new();
        
        public event Action OnUnitStartTurn;
        public event Action OnUnitTurnTaken;
        
        public void NextUnit(BattleUnit battleUnit = null) {
            if (ActivatedUnit) return;
            battleUnit ??= SelectedUnit;
            
            var index = Units.IndexOf(battleUnit);
            index++;
            if (index >= Units.Count) index = 0;
            
            if (Units[index].TurnTaken || Units[index].UnitStatus == UnitStatus.Eliminated) {
                NextUnit(Units[index]);
                return;
            }
            
            SetSelectedUnit(Units[index]);
        }
        
        public void EndUnitTurn() {
            SelectedUnit.TurnTaken = true;
            SelectedUnit.SetSelected(false); 
            ActivatedUnit = false;
            OnUnitTurnTaken?.Invoke();
        }

        public void ResetTurns() {
            Units.ForEach(u => u.ResetTurn());
            SelectedUnit = null;
        }
        
        public void AddOrUpdateEnemyLastSeen(BattleUnit battleUnit, Tile tile) => EnemyLastSeen[battleUnit] = tile;
        public bool AllUnitsTurnTaken() => Units.Where(u => u.UnitStatus == UnitStatus.Active).All(u => u.TurnTaken);
        public void TakeTurn() => OnUnitStartTurn?.Invoke();

        private void SetSelectedUnit(BattleUnit battleUnit) {
            SelectedUnit?.SetSelected(false);
            SelectedUnit = battleUnit;
            SelectedUnit.SetSelected(true);
        }

        
    }
}