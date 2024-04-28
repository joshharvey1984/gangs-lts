using System.Collections.Generic;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Data {
    public class Faction : Entity {
        public static List<Faction> All { get; set; } = new();
        
        public Texture2D Logo { get; set; }
        public Color Color { get; set; }
        public List<AttributeModifier> AttributeModifiers { get; set; }
        
        public Faction(FactionDto dto) : base(dto) {
            All.Add(this);
        }

        public void Create(FactionDto dto) {
            Logo = Resources.Load<Texture2D>($"Clans/{dto.logo}");
            Color = new Color(dto.color[0], dto.color[1], dto.color[2]);
            AttributeModifiers = new List<AttributeModifier>();
            foreach (var attributeModifier in dto.attributeModifiers) {
                AttributeModifiers.Add(new AttributeModifier {
                    Attribute = (UnitAttribute) System.Enum.Parse(typeof(UnitAttribute), attributeModifier.attribute),
                    Modifier = attributeModifier.modifier
                });
            }
        }
        
        public List<AttributeModifier> GetAttributeModifiersByAttribute(UnitAttribute attribute) {
            return AttributeModifiers.FindAll(m => m.Attribute == attribute);
        }
    }
}