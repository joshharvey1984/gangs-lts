using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class ClanDto : EntityDto {
        public AttributeModifierDto[] attributeModifiers;
        public static ClanDto CreateFromJson(string json) => JsonUtility.FromJson<ClanDto>(json);
    }
}