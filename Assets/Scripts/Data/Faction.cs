using System.Collections.Generic;
using System.Linq;
using Gangs.Core;
using Gangs.Data.DTO;
using UnityEngine;

namespace Gangs.Data {
    public class Faction : Entity {
        public static List<Faction> All { get; set; } = new();
        
        public bool Playable { get; set; }
        public Texture2D Logo { get; set; }
        public Color Color { get; set; }
        public List<Unit> Units { get; set; }
        public List<AttributeModifier> AttributeModifiers { get; set; }
        
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
                    AttributeType = (UnitAttributeType) System.Enum.Parse(typeof(UnitAttributeType), attributeModifier.attribute),
                    Modifier = attributeModifier.modifier
                });
            }
        }
    }
}