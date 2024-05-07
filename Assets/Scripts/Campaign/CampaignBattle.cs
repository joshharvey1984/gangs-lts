using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Data;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;

namespace Gangs.Campaign {
    public class CampaignBattle : IBattle {
        CampaignTerritory Territory { get; }
        public BattleBase BattleBase { get; }
        
        public event Action<CampaignSquad> OnEndBattle;
        
        public CampaignBattle(CampaignTerritory territory) {
            Territory = territory;
            BattleBase = new BattleBase();
            if (territory.Map == null) Debug.LogError("Map is null");
            BattleBase.CreateGrid(territory.Map);
            var squads = CreateSquads(territory.Squads);
            squads.ForEach(squad => BattleBase.AddSquad(squad));
            var spawnPositions = SpawnSquads(territory.Map);
            BattleBase.SpawnSquad(spawnPositions);
            BattleBase.OnEndGame += EndBattle;
        }
        
        public void StartBattle() {
            BattleBase.StartBattle();
        }
        
        public List<BattleUnit> GetUnits() {
            return BattleBase.Squads.SelectMany(squad => squad.Units).ToList();
        }

        public void MoveUnit(BattleUnit unit, Tile tile) {
            BattleBase.MoveUnit(unit, tile);
        }

        private void EndBattle(BattleSquad battleSquad) {
            var unit = battleSquad.Units.FirstOrDefault()!.Unit;
            var victor = Territory.Squads.FirstOrDefault(squad => squad.Units.Contains(unit));
            OnEndBattle?.Invoke(victor);
        }

        // TODO: Implement spawning squads in Map Editor
        private List<List<GridPosition>> SpawnSquads(Map map) {
            var maxX = map.Tiles.Max(t => t.X);
            var maxZ = map.Tiles.Max(t => t.Z);
            
            var spawnPositions1 = new List<GridPosition> {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0),
                new(3, 0, 0),
                new(4, 0, 0),
                new(5, 0, 0)
            };
            
            var spawnPositions2 = new List<GridPosition> {
                new(maxX, 0, maxZ),
                new(maxX - 1, 0, maxZ),
                new(maxX - 2, 0, maxZ),
                new(maxX - 3, 0, maxZ),
                new(maxX - 4, 0, maxZ),
                new(maxX - 5, 0, maxZ)
            };
            
            return new List<List<GridPosition>> {
                spawnPositions1,
                spawnPositions2
            };
        }
        
        private List<BattleSquad> CreateSquads(List<CampaignSquad> territorySquads) {
            var squads = new List<BattleSquad>();

            territorySquads.ForEach(entity => {
                var squad = new BattleSquad();

                entity.Units.ForEach(unit => {
                    var battleUnit = new BattleUnit(unit, BattleBase.Grid);
                    battleUnit.OnUnitEliminated += BattleBase.Grid.RemoveUnit;
                    squad.Units.Add(battleUnit);
                });
                
                squads.Add(squad);
            });
            
            return squads;
        }
    }
}