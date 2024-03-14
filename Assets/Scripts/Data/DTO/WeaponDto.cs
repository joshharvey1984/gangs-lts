using UnityEngine;

namespace Gangs.Data.DTO {
    public class WeaponDto : EquipmentDto {
        public int damage;
        public int shortRange;
        public int shortRangeModifier;
        public int longRangeModifier;
        
        public static WeaponDto CreateFromJson(string json) => JsonUtility.FromJson<WeaponDto>(json);
    }
}