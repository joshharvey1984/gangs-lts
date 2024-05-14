using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle.Grid;
using Gangs.Core;
using Gangs.Data;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Battle {
    public class BattleBase {
        public BattleGrid Grid { get; private set; }
        public List<BattleSquad> Squads { get; } = new();
        public BattleSquad ActiveSquad { get; private set; }
        private int RoundNumber { get; set; } = 1;
        
        // replace with Battle status enum
        public enum BattleStatus {
            Setup,
            Active,
            End
        }
        
        public BattleStatus Status { get; private set; } = BattleStatus.Setup;
        
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
                    // catch out of range exception
                    try {
                        var spawnPosition = spawnPositionGroups[i][j];
                    } catch (Exception e) {
                        Debug.LogError(e);
                        return;
                    }
                    var gridUnit = Grid.Grid.AddUnit(spawnPositionGroups[i][j]);
                    Squads[i].Units[j].GridUnit = gridUnit;
                }
            }
        }
        
        public void StartBattle() {
            Status = BattleStatus.Active;
            NextTurn();
        }
        
        public IEnumerable<BattleUnit> GetActiveEnemyUnits(BattleUnit battleUnit) => 
            GetEnemyUnits(battleUnit).Where(u => u.UnitStatus != UnitStatus.Eliminated);

        private IEnumerable<BattleUnit> GetEnemyUnits(BattleUnit battleUnit) => 
            Squads.Where(sq => !sq.Units.Contains(battleUnit)).SelectMany(sq => sq.Units).ToList();
        
        public void MoveUnit(BattleUnit unit, Tile tile) => Grid.MoveUnit(unit, tile);
        
        public CoverType GetCoverType(GridPosition gridPosition, CardinalDirection direction) => 
            Grid.GetCoverType(gridPosition, direction);
        
        private void NextTurn() {
            ActiveSquad = GetNextSquadTurn();
            ActiveSquad.NextUnit();
            ActiveSquad.TakeTurn();
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
            Debug.Log($"End of round {RoundNumber}");
            Squads.ForEach(s => s.ResetTurns());
            RoundNumber++;
            
            if (RoundNumber > 7) {
                EndGame(null);
            }
        }
        
        public bool CheckForEndGame() {
            if (Status == BattleStatus.End) return true;
            if (Squads.Any(s => s.Units.All(u => u.UnitStatus == UnitStatus.Eliminated))) {
                EndGame(Squads.FirstOrDefault(s => s.Units.Any(u => u.UnitStatus != UnitStatus.Eliminated)));
                Status = BattleStatus.End;
                return true;
            }
            
            return false;
        }

        private void EndGame(BattleSquad victor) => OnEndGame?.Invoke(victor);
    }
}