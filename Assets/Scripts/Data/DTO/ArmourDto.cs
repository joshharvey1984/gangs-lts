using UnityEngine;

namespace Gangs.Data.DTO {
    public class ArmourDto : EquipmentDto {
        public int damageReduction;
        
        public static ArmourDto CreateFromJson(string json) => JsonUtility.FromJson<ArmourDto>(json);
    }
}