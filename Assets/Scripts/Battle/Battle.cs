using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle.AI;
using Gangs.Battle.Grid;
using Gangs.Data;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Battle {
    public class Battle {
        public BattleGrid Grid { get; private set; }
        public List<BattleSquad> Squads { get; } = new();
        public BattleSquad ActiveSquad { get; private set; }
        public int RoundNumber { get; set; } = 1;
        
        public void CreateGrid(Map map) {
            Grid = new BattleGrid(map);
            Grid.OnGetUnit += GetUnit;
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
            ActiveSquad.NextUnit();
            BattleAI.TakeTurn(ActiveSquad.SelectedUnit, this);
            EndSquadTurn();
        }
        
        private BattleUnit GetUnit(GridUnit gridUnit) => 
            Squads.SelectMany(s => s.Units).FirstOrDefault(u => u.GridUnit == gridUnit);
        
        public Tile GetSoldierTile(BattleUnit battleUnit) => Grid.Grid.GetTileByGridUnit(battleUnit.GridUnit);
        public BattleUnit FindUnit(GridUnit hoverTileGridUnit) => ActiveSquad.Units.FirstOrDefault(unit => unit.GridUnit == hoverTileGridUnit);
        
        public void EndSquadTurn() {
            if (Squads.All(s => s.AllUnitsTurnTaken)) EndRound(); 

            ActiveSquad = GetNextSquadTurn();
            ActiveSquad.NextUnit();
            BattleAI.TakeTurn(ActiveSquad.SelectedUnit, this);
            CheckForEndGame();
            EndSquadTurn();
        }
        
        private BattleSquad GetNextSquadTurn(BattleSquad battleSquad = null) {
            battleSquad ??= ActiveSquad;
            var nextSquadIndex = Squads.IndexOf(battleSquad) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken ? GetNextSquadTurn(Squads[nextSquadIndex]) : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Debug.Log($"End of Round {RoundNumber}");
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

        public IEnumerable<BattleUnit> GetEnemyUnits(BattleUnit battleUnit) => 
            Squads.Where(sq => !sq.Units.Contains(battleUnit)).SelectMany(sq => sq.Units).ToList();
    }
}