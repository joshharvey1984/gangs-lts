using System;
using UnityEngine;

namespace Gangs.Data.DTO {
    [Serializable]
    public class UnitDto : EntityDto {
        public BaseAttributeDto[] baseAtrributes;
        public ModifierDto[] modifiers;
        
        public static UnitDto CreateFromJson(string json) => JsonUtility.FromJson<UnitDto>(json);
    }
    
    [Serializable]
    public class ModifierDto {
        public int level;
        public string type;
        public string attribute;
        public string value;
    }
}