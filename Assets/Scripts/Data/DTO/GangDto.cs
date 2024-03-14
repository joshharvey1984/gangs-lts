using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class GangDto : EntityDto {
        public string clanId;
        public string[] fighterIds;
        
        public static GangDto CreateFromJson(string json) => JsonUtility.FromJson<GangDto>(json);
    }
}