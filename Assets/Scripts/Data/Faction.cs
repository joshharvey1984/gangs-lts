using System.Collections.Generic;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Data {
    public class Faction : Entity {
        // TODO: Figure some way of neutral spawn behavior
        // ie Creatures randomly spawn in the campaign map
        // Events that spawn creatures or bandits
        public static List<Faction> All { get; } = new();
        
        public bool Playable { get; private set; }
        public Texture2D Logo { get; private set; }
        public Color Color { get; private set; }
        public List<Unit> Units { get; private set; }
        public List<AttributeModifier> AttributeModifiers { get; private set; }
        
        public Faction(FactionDto dto) : base(dto) {
            All.Add(this);
        }

        public void Create(FactionDto dto) {
            Playable = dto.playable;
            Logo = Resources.Load<Texture2D>($"Clans/{dto.logo}");
            Color = new Color(dto.color[0], dto.color[1], dto.color[2]);
            
            Units = new List<Unit>();
            foreach (var unit in dto.units) {
                Units.Add(Unit.All.Find(u => u.ID == unit));
            }
            
            AttributeModifiers = new List<AttributeModifier>();
            foreach (var attributeModifier in dto.attributeModifiers) {
                AttributeModifiers.Add(new AttributeModifier {
                    AttributeType = AttributeModifier.GetAttribute(attributeModifier.attribute),
                    Modifier = attributeModifier.modifier
                });
            }
        }
    }
}