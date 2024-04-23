using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class TerritoryDto : EntityDto {
        public TerritoryResourceDto[] territoryResources;
        
        public static TerritoryDto CreateFromJson(string json) => JsonUtility.FromJson<TerritoryDto>(json);
    }

    [Serializable]
    public class TerritoryResourceDto {
        public string resourceId;
        public int amount;
        public int cooldown;
    }
}