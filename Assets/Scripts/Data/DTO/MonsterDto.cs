using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class MonsterDto : EntityDto {
        public int movement;
        public int aim;
        public int closeQuarters;
        public int hitPoints;
        public int actionPoints;
        
        public static MonsterDto CreateFromJson(string json) => JsonUtility.FromJson<MonsterDto>(json);
    }
}