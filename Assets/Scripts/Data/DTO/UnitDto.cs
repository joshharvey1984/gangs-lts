using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class UnitDto : EntityDto {
        public string clanId;
        public int level;
        
        public static UnitDto CreateFromJson(string json) => JsonUtility.FromJson<UnitDto>(json);
    }
}