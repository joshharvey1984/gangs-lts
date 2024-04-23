using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Ruleset : Entity {
        public static List<Ruleset> All { get; set; } = new();
        
        public List<Territory> ValidTerritories { get; set; }
        public List<StartingTerritory> StartingTerritories { get; set; }
        
        public Ruleset(EntityDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(RulesetDto dto) {
            ValidTerritories = new List<Territory>();
            foreach (var territoryDto in dto.validTerritories) {
                ValidTerritories.Add(Territory.All.Find(t => t.ID == territoryDto));
            }
            StartingTerritories = new List<StartingTerritory>();
            foreach (var startingTerritoryDto in dto.startingTerritories) {
                StartingTerritories.Add(new StartingTerritory(startingTerritoryDto));
            }
        }
    }

    public class StartingTerritory {
        public Territory Territory { get; set; }
        public bool Headquarters { get; set; }
        
        public StartingTerritory(StartingTerritoryDto dto) {
            Territory = Territory.All.Find(t => t.ID == dto.territoryId);
            Headquarters = dto.headquarters;
        }
    }
}