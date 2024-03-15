using System.Collections.Generic;
using System.Linq;
using Gangs.Managers;

namespace Gangs {
    public class Squad {
        public readonly List<Unit> Units = new();
        
        public Unit SelectedUnit { get; set; }
        public bool ActivatedUnit;
        
        public bool AllUnitsTurnTaken => Units.All(u => u.TurnTaken);

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
            
            InputManager.Instance.SelectUnit(SelectedUnit);
        }
        
        public void EndUnitTurn() {
            SelectedUnit.TurnTaken = true;
            SelectedUnit.SetSelected(false);
            SelectedUnit = null;
            ActivatedUnit = false;
            GameManager.Instance.EndSquadTurn();
        }

        public void ResetTurns() => Units.ForEach(u => u.ResetTurn());
    }
    
    public class PlayerSquad : Squad {
        
    }
    
    public class AISquad : Squad {
        
    }
}