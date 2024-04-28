using System.Collections.Generic;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Monster : Entity {
        public static List<Monster> All { get; set; } = new();
        
        public List<Attribute> Attributes { get; private set; }
        
        public Monster(EntityDto dto) : base(dto) {
            All.Add(this);
        }
        
        public void Create(MonsterDto dto) {
            Attributes = new List<Attribute> {
                new() { BaseValue = dto.movement, UnitAttribute = UnitAttribute.Movement, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.aim, UnitAttribute = UnitAttribute.Aim, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.closeQuarters, UnitAttribute = UnitAttribute.CloseQuarters, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.hitPoints, UnitAttribute = UnitAttribute.HitPoints, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.actionPoints, UnitAttribute = UnitAttribute.ActionPoints, Modifiers = new List<AttributeModifier>()}
            };
        }
    }
}