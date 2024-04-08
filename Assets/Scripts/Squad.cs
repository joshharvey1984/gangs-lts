using System.Collections.Generic;
using System.Linq;
using Gangs.AI;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs {
    public class Squad {
        public readonly List<Unit> Units = new();
        public List<Unit> ActiveUnits => Units.Where(u => u.Status == Status.Active).ToList();
        
        public Unit SelectedUnit { get; set; }
        public bool ActivatedUnit;
        
        public bool AllUnitsTurnTaken => Units.Where(u => u.Status == Status.Active).All(u => u.TurnTaken);
        
        public Dictionary<Unit, Tile> EnemyLastSeen = new();

        private void SetSelectedUnit(Unit unit) {
            SelectedUnit?.SetSelected(false);
            SelectedUnit = unit;
            SelectedUnit.SetSelected(true);
        }
        
        public void NextUnit(Unit unit = null) {
            if (ActivatedUnit) return;
            if (unit == null) unit = SelectedUnit;
            
            if (AllUnitsTurnTaken) {
                GameManager.Instance.EndSquadTurn();
                return;
            }
            
            var index = Units.IndexOf(unit);
            index++;
            if (index >= Units.Count) index = 0;
            
            if (Units[index].TurnTaken || Units[index].Status == Status.Eliminated) {
                NextUnit(Units[index]);
                return;
            }
            
            SetSelectedUnit(Units[index]);
            
            if (SelectedUnit.IsPlayerControlled)
                InputManager.Instance.SelectUnit(SelectedUnit);
            else
                EnemyAI.TakeTurn(SelectedUnit);
        }
        
        public void EndUnitTurn() {
            SelectedUnit.TurnTaken = true;
            SelectedUnit.SetSelected(false); 
            ActivatedUnit = false;
            GameManager.Instance.EndSquadTurn();
        }

        public void ResetTurns() {
            Units.ForEach(u => u.ResetTurn());
            SelectedUnit = null;
        }
        
        public void AddOrUpdateEnemyLastSeen(Unit unit, Tile tile) => EnemyLastSeen[unit] = tile;
    }
    
    public class PlayerSquad : Squad { }
    
    public class AISquad : Squad {
        public EnemyAIWeightings Weightings;
        
        public AISquad(EnemyAIWeightings weightings) {
            Weightings = weightings;
        }
    }
}