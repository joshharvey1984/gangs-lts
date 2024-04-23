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
                new() { BaseValue = dto.movement, FighterAttribute = FighterAttribute.Movement, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.aim, FighterAttribute = FighterAttribute.Aim, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.closeQuarters, FighterAttribute = FighterAttribute.CloseQuarters, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.hitPoints, FighterAttribute = FighterAttribute.HitPoints, Modifiers = new List<AttributeModifier>()},
                new() { BaseValue = dto.actionPoints, FighterAttribute = FighterAttribute.ActionPoints, Modifiers = new List<AttributeModifier>()}
            };
        }
    }
}