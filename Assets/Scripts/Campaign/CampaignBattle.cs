using System.Collections.Generic;
using Gangs.AI;
using Gangs.Battle;
using Gangs.Data;
using Gangs.Grid;

namespace Gangs.Campaign {
    public class CampaignBattle {
        public CampaignBattleType BattleType { get; }
        
        public CampaignBattle(CampaignTerritory territory, CampaignBattleType battleType) {
            BattleType = battleType;
            var battle = new Battle.Battle();
            battle.CreateGrid(territory.Map);
            var squads = CreateSquads(territory.Squads, battleType);
            squads.ForEach(squad => battle.AddSquad(squad));
            var spawnPositions = SpawnSquads(territory.Map);
            battle.SpawnSquad(spawnPositions);
        }

        // TODO: Implement spawning squads in Map Editor
        private List<List<GridPosition>> SpawnSquads(Map map) {
            var spawnPositions1 = new List<GridPosition> {
                new(0, 0, 0),
                new(1, 0, 0),
                new(2, 0, 0),
                new(3, 0, 0)
            };
            
            var spawnPositions2 = new List<GridPosition> {
                new(map.Tiles.Count - 1, 0, map.Tiles.Count - 1),
                new(map.Tiles.Count - 2, 0, map.Tiles.Count - 1),
                new(map.Tiles.Count - 3, 0, map.Tiles.Count - 1),
                new(map.Tiles.Count - 4, 0, map.Tiles.Count - 1)
            };
            
            return new List<List<GridPosition>> {
                spawnPositions1,
                spawnPositions2
            };
        }
        
        private List<BattleSquad> CreateSquads(List<CampaignSquad> territoryEntities, CampaignBattleType battleType) {
            var squads = new List<BattleSquad>();
            
            territoryEntities.ForEach(entity => {
                var aiSquad = battleType == CampaignBattleType.Auto;
                if (aiSquad) {
                    var squad = new AIBattleSquad(new BattleAIWeightings() {
                        CanFlankWeight = 1,
                        DistanceCheckWeight = 1,
                        FullCoverWeight = 1,
                        HalfCoverWeight = 1,
                        HeightAdvantageWeight = 1,
                        IsFlankedWeight = 1,
                        RemainingActionPointWeight = 1
                    }, entity);

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