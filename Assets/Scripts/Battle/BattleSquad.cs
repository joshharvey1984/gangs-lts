﻿using System.Collections.Generic;
using System.Linq;
using Gangs.AI;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs.Battle {
    public abstract class BattleSquad {
        public readonly List<BattleUnit> Units = new();
        public List<BattleUnit> ActiveUnits => Units.Where(u => u.Status == Status.Active).ToList();
        
        public BattleUnit SelectedBattleUnit { get; set; }
        public bool ActivatedUnit;
        
        public bool AllUnitsTurnTaken => Units.Where(u => u.Status == Status.Active).All(u => u.TurnTaken);
        
        public Dictionary<BattleUnit, Tile> EnemyLastSeen = new();

        private void SetSelectedUnit(BattleUnit battleUnit) {
            SelectedBattleUnit?.SetSelected(false);
            SelectedBattleUnit = battleUnit;
            SelectedBattleUnit.SetSelected(true);
        }
        
        public void NextUnit(BattleUnit battleUnit = null) {
            if (ActivatedUnit) return;
            if (battleUnit == null) battleUnit = SelectedBattleUnit;
            
            if (AllUnitsTurnTaken) {
                BattleManager.Instance.EndSquadTurn();
                return;
            }
            
            var index = Units.IndexOf(battleUnit);
            index++;
            if (index >= Units.Count) index = 0;
            
            if (Units[index].TurnTaken || Units[index].Status == Status.Eliminated) {
                NextUnit(Units[index]);
                return;
            }
            
            SetSelectedUnit(Units[index]);
            
            if (SelectedBattleUnit.IsPlayerControlled)
                InputManager.Instance.SelectUnit(SelectedBattleUnit);
            else
                EnemyAI.TakeTurn(SelectedBattleUnit);
        }
        
        public void EndUnitTurn() {
            SelectedBattleUnit.TurnTaken = true;
            SelectedBattleUnit.SetSelected(false); 
            ActivatedUnit = false;
            BattleManager.Instance.EndSquadTurn();
        }

        public void ResetTurns() {
            Units.ForEach(u => u.ResetTurn());
            SelectedBattleUnit = null;
        }
        
        public void AddOrUpdateEnemyLastSeen(BattleUnit battleUnit, Tile tile) => EnemyLastSeen[battleUnit] = tile;
    }
    
    public class PlayerBattleSquad : BattleSquad { }
    
    public class AIBattleSquad : BattleSquad {
        public EnemyAIWeightings Weightings;
        
        public AIBattleSquad(EnemyAIWeightings weightings) {
            Weightings = weightings;
        }
    }
}