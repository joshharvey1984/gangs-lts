using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Equipment : Entity {
        public int Price { get; set; }
        public EquipmentType EquipmentType { get; set; }
        
        public Equipment(EquipmentDto dto) : base(dto) {
            Price = dto.price;
            EquipmentType = (EquipmentType) dto.typeId;
        }
    }
    
    public enum EquipmentType {
        Melee = 0,
        Pistol = 1,
        Gun = 2,
        Armor = 3,
        Consumable = 4,
    }
}