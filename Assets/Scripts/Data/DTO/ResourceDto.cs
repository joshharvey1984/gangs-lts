using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class ResourceDto : EntityDto {
        public static ResourceDto CreateFromJson(string json) => JsonUtility.FromJson<ResourceDto>(json);
    }
}