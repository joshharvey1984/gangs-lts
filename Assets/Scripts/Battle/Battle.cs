using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle.Grid;
using Gangs.Data;
using Gangs.Grid;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Battle {
    public class Battle {
        public BattleGrid Grid { get; private set; }
        public List<BattleSquad> Squads { get; set; } = new();
        public BattleSquad ActiveSquad { get; set; }
        public int RoundNumber { get; set; } = 1;
        
        public void CreateGrid(Map map) {
            Grid = new BattleGrid(map);
        }
        
        public void AddSquad(BattleSquad squad) {
            Squads.Add(squad);
        }

        public void SpawnSquad(List<List<GridPosition>> spawnPositionGroups) {
            for (var i = 0; i < Squads.Count; i++) {
                for (var j = 0; j < Squads[i].Units.Count; j++) {
                    var gridUnit = Grid.Grid.AddUnit(spawnPositionGroups[i][j]);
                    Squads[i].Units[j].GridUnit = gridUnit;
                }
            }
        }
        
        public void StartTurn() {
            ActiveSquad = Squads[0];
        }
        
        public BattleUnit SelectedBattleUnit => ActiveSquad.SelectedBattleUnit;
        public bool ActivatedUnit => ActiveSquad.ActivatedUnit;
        
        private void MoveUnit(GridPosition gridPosition) {
            var movingUnit = ActiveSquad.SelectedBattleUnit;
            Grid.Grid.MoveUnit(movingUnit.GridUnit, gridPosition);
            
            var movedUnitTile = Grid.Grid.GetTile(gridPosition);
            var enemySquads = Squads.Where(s => s != ActiveSquad).ToList();
            var enemyUnits = enemySquads.SelectMany(s => s.Units).Where(u => u.Status != Status.Eliminated).ToList();

            foreach (var enemyUnit in enemyUnits) {
                var enemyTile = GetSoldierTile(enemyUnit);
                var enemySquad = enemySquads.FirstOrDefault(s => s.Units.Contains(enemyUnit));
                if (enemyTile.LineOfSightGridPositions.Contains(movedUnitTile.GridPosition)) {
                    ActiveSquad.AddOrUpdateEnemyLastSeen(enemyUnit, enemyTile);
                    enemySquad!.AddOrUpdateEnemyLastSeen(movingUnit, movedUnitTile);
                }
            }
        }
        
        public Tile GetSoldierTile(BattleUnit battleUnit) => Grid.Grid.GetTileByGridUnit(battleUnit.GridUnit);
        public void NextUnit() => ActiveSquad.NextUnit();
        public BattleUnit FindUnit(GridUnit hoverTileGridUnit) => ActiveSquad.Units.FirstOrDefault(unit => unit.GridUnit == hoverTileGridUnit);
        
        public void EndSquadTurn() {
            if (Squads.All(s => s.AllUnitsTurnTaken)) EndRound(); 

            ActiveSquad = GetNextSquadTurn();
            ActiveSquad.NextUnit();
        }
        
        private BattleSquad GetNextSquadTurn(BattleSquad battleSquad = null) {
            battleSquad ??= ActiveSquad;
            var nextSquadIndex = Squads.IndexOf(battleSquad) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken ? GetNextSquadTurn(Squads[nextSquadIndex]) : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Squads.ForEach(s => s.ResetTurns());
            RoundNumber++;
            
            if (RoundNumber > 100) {
                EndGame();
            }
        }
        
        public void CheckForEndGame() {
            if (Squads.Any(s => s.Units.All(u => u.Status == Status.Eliminated))) {
                EndGame();
            }
        }
        
        public void EndGame() {
            throw new EndGameException();
        }
        
        public class EndGameException : Exception {
            public EndGameException() : base("Game Over") { }
        }
    }
}