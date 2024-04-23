using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class FighterDto : EntityDto {
        public string clanId;
        public int level;
        
        public static FighterDto CreateFromJson(string json) => JsonUtility.FromJson<FighterDto>(json);
    }
}