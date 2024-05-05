using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle.AI;
using Gangs.Campaign;
using Gangs.Grid;

namespace Gangs.Battle {
    public abstract class BattleSquad {
        public readonly List<BattleUnit> Units = new();
        public List<BattleUnit> ActiveUnits => Units.Where(u => u.Status == Status.Active).ToList();
        
        public BattleUnit SelectedUnit { get; set; }
        public bool ActivatedUnit;
        
        
        public Dictionary<BattleUnit, Tile> EnemyLastSeen = new();
        
        public event Action OnUnitTurnTaken;
        public event Action OnAllUnitsTurnTaken;
        
        public void NextUnit(BattleUnit battleUnit = null) {
            if (ActivatedUnit) return;
            if (battleUnit == null) battleUnit = SelectedUnit;
            
            if (AllUnitsTurnTaken()) {
                OnAllUnitsTurnTaken?.Invoke();
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
        public bool AllUnitsTurnTaken() => Units.Where(u => u.Status == Status.Active).All(u => u.TurnTaken);

        private void SetSelectedUnit(BattleUnit battleUnit) {
            SelectedUnit?.SetSelected(false);
            SelectedUnit = battleUnit;
            SelectedUnit.SetSelected(true);
        }
    }
    
    public class PlayerBattleSquad : BattleSquad { }
    
    public class AIBattleSquad : BattleSquad {
        public BattleAIWeightings Weightings;
        
        public AIBattleSquad(BattleAIWeightings weightings, CampaignSquad entity) {
            Weightings = weightings;
        }
    }
}