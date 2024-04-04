using System.Collections.Generic;
using System.Linq;
using Gangs.AI;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs {
    public class Squad {
        public readonly List<Unit> Units = new();
        
        public Unit SelectedUnit { get; set; }
        public bool ActivatedUnit;
        
        public bool AllUnitsTurnTaken => Units.All(u => u.TurnTaken);
        
        public Dictionary<Unit, Tile> EnemyLastSeen = new();

        private void SetSelectedUnit(Unit unit) {
            SelectedUnit?.SetSelected(false);
            SelectedUnit = unit;
            SelectedUnit.SetSelected(true);
        }
        
        public void NextUnit() {
            if (ActivatedUnit) return;
            
            var index = Units.IndexOf(SelectedUnit);
            index++;
            if (index >= Units.Count) index = 0;
            
            SetSelectedUnit(Units[index]);
            
            if (SelectedUnit.TurnTaken) {
                NextUnit();
                return;
            }
            
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