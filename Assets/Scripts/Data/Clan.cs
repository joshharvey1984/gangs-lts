using System.Collections.Generic;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Data {
    public class Clan : Entity {
        public static List<Clan> All { get; set; } = new();
        
        public Texture2D Logo { get; set; }
        public List<AttributeModifier> AttributeModifiers { get; set; }
        
        public Clan(ClanDto dto) : base(dto) {
            All.Add(this);
        }

        public void Create(ClanDto dto) {
            Logo = Resources.Load<Texture2D>($"Clans/{dto.logo}");
            AttributeModifiers = new List<AttributeModifier>();
            foreach (var attributeModifier in dto.attributeModifiers) {
                AttributeModifiers.Add(new AttributeModifier {
                    Attribute = (FighterAttribute) System.Enum.Parse(typeof(FighterAttribute), attributeModifier.attribute),
                    Modifier = attributeModifier.modifier
                });
            }
        }
        
        public List<AttributeModifier> GetAttributeModifiersByAttribute(FighterAttribute attribute) {
            return AttributeModifiers.FindAll(m => m.Attribute == attribute);
        }
    }
}