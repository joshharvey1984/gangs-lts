using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Data;
using Gangs.Grid;

namespace Gangs.Campaign {
    public class CampaignBattle : IBattle {
        CampaignTerritory Territory { get; }
        public CampaignBattleType BattleType { get; }
        public Battle.Battle Battle { get; }
        
        public event Action<CampaignSquad> OnEndBattle;
        
        public CampaignBattle(CampaignTerritory territory, CampaignBattleType battleType) {
            Territory = territory;
            BattleType = battleType;
            Battle = new Battle.Battle();
            Battle.CreateGrid(territory.Map);
            var squads = CreateSquads(territory.Squads, battleType);
            squads.ForEach(squad => Battle.AddSquad(squad));
            var spawnPositions = SpawnSquads(territory.Map);
            Battle.SpawnSquad(spawnPositions);
            Battle.OnEndGame += EndBattle;
        }
        
        public void StartBattle() {
            Battle.StartBattle();
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
        
        // TODO: Implement AI Wieghtings in unit data
        private List<BattleSquad> CreateSquads(List<CampaignSquad> territoryEntities, CampaignBattleType battleType) {
            var squads = new List<BattleSquad>();
            
            territoryEntities.ForEach(entity => {
                var aiSquad = battleType == CampaignBattleType.Auto;
                if (aiSquad) {
                    var squad = new AIBattleSquad(new BattleAIWeightings {
                        CanFlankWeight = 1,
                        DistanceCheckWeight = 1,
                        FullCoverWeight = 1,
                        HalfCoverWeight = 1,
                        HeightAdvantageWeight = 1,
                        IsFlankedWeight = 1,
                        RemainingActionPointWeight = 1
                    }, entity);
                    
                    entity.Units.ForEach(unit => {
                        var battleUnit = new BattleUnit(unit, Battle.Grid);
                        battleUnit.OnUnitEliminated += Battle.Grid.RemoveUnit;
                        squad.Units.Add(battleUnit);
                    });
                    squads.Add(squad);
                }
                else {
                    var squad = new PlayerBattleSquad();
                    squads.Add(squad);
                }
            });

            return squads;
        }
    }
    
    public enum CampaignBattleType {
        Auto,
        Manual
    }
}