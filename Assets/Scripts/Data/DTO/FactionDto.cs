using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class FactionDto : EntityDto {
        public bool playable;
        public string logo;
        public float[] color;
        public string[] units;
        public AttributeModifierDto[] attributeModifiers;
        public static FactionDto CreateFromJson(string json) => JsonUtility.FromJson<FactionDto>(json);
    }
}