using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class RulesetDto : EntityDto {
        public string[] validTerritories;
        public StartingTerritoryDto[] startingTerritories;
        public int otherRandomTerritories;
        
        public static RulesetDto CreateFromJson(string json) => JsonUtility.FromJson<RulesetDto>(json);
    }

    [Serializable]
    public class StartingTerritoryDto {
        public string territoryId;
        public bool headquarters;
    }
}