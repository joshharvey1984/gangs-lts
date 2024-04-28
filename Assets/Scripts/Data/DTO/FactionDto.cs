using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class FactionDto : EntityDto {
        public string logo;
        public float[] color;
        public AttributeModifierDto[] attributeModifiers;
        public static FactionDto CreateFromJson(string json) => JsonUtility.FromJson<FactionDto>(json);
    }
}