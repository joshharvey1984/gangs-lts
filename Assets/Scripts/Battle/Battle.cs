using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle.AI;
using Gangs.Battle.Grid;
using Gangs.Data;
using Gangs.Grid;

namespace Gangs.Battle {
    public class Battle {
        public BattleGrid Grid { get; private set; }
        private List<BattleSquad> Squads { get; } = new();
        public BattleSquad ActiveSquad { get; private set; }
        private int RoundNumber { get; set; } = 1;
        
        public event Action<BattleSquad> OnEndGame;
        
        public void CreateGrid(Map map) {
            Grid = new BattleGrid(map);
            Grid.OnGetUnit += GetUnit;
        }
        
        public void AddSquad(BattleSquad squad) {
            Squads.Add(squad);
            squad.OnUnitTurnTaken += EndSquadTurn;
        }

        public void SpawnSquad(List<List<GridPosition>> spawnPositionGroups) {
            for (var i = 0; i < Squads.Count; i++) {
                for (var j = 0; j < Squads[i].Units.Count; j++) {
                    var gridUnit = Grid.Grid.AddUnit(spawnPositionGroups[i][j]);
                    Squads[i].Units[j].GridUnit = gridUnit;
                }
            }
        }
        
        public void StartBattle() {
            NextTurn();
        }
        
        public IEnumerable<BattleUnit> GetEnemyUnits(BattleUnit battleUnit) => 
            Squads.Where(sq => !sq.Units.Contains(battleUnit)).SelectMany(sq => sq.Units).ToList();

        private void NextTurn() {
            ActiveSquad = GetNextSquadTurn();
            ActiveSquad.NextUnit();
            BattleAI.TakeTurn(ActiveSquad.SelectedUnit, this);
        }
        
        private BattleUnit GetUnit(GridUnit gridUnit) => 
            Squads.SelectMany(s => s.Units).FirstOrDefault(u => u.GridUnit == gridUnit);

        private void EndSquadTurn() {
            if (CheckForEndGame()) return;
            if (Squads.All(s => s.AllUnitsTurnTaken())) EndRound(); 
            NextTurn();
        }
        
        private BattleSquad GetNextSquadTurn(BattleSquad battleSquad = null) {
            battleSquad ??= ActiveSquad;
            var nextSquadIndex = Squads.IndexOf(battleSquad) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken() ? GetNextSquadTurn(Squads[nextSquadIndex]) : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Squads.ForEach(s => s.ResetTurns());
            RoundNumber++;
            
            if (RoundNumber > 20) {
                EndGame(null);
            }
        }
        
        private bool CheckForEndGame() {
            if (Squads.Any(s => s.Units.All(u => u.Status == Status.Eliminated))) {
                EndGame(Squads.FirstOrDefault(s => s.Units.Any(u => u.Status != Status.Eliminated)));
                return true;
            }
            
            return false;
        }

        private void EndGame(BattleSquad victor) {
            OnEndGame?.Invoke(victor);
        }
    }
}