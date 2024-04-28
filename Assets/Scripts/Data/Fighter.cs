using System.Collections.Generic;
using System.Linq;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Fighter: Entity {
        public static List<Fighter> All { get; set; } = new();
        
        public Faction Faction { get; set; }
        public int Level { get; set; }
        public FighterClass FighterClass { get; set; }
        public List<Attribute> Attributes { get; set; }
        
        public Fighter(UnitDto dto) : base(dto) { 
            All.Add(this);
        }
        
        public void Create(UnitDto dto) {
            Faction = Faction.All.Find(c => c.ID == dto.clanId);
            Level = dto.level;
            FighterClass = Level switch {
                < 6 => FighterClass.Juve,
                < 11 => FighterClass.Ganger,
                < 16 => FighterClass.Veteran,
                _ => FighterClass.Champion
            };

            Attributes = new List<Attribute> {
                SetAttribute(UnitAttribute.Movement, 5),
                SetAttribute(UnitAttribute.Aim, 2),
                SetAttribute(UnitAttribute.CloseQuarters, 2),
                SetAttribute(UnitAttribute.HitPoints, 5),
                SetAttribute(UnitAttribute.ActionPoints, 2)
            };
        }

        protected Attribute SetAttribute(UnitAttribute attribute, int baseValue) =>
            new() {
                UnitAttribute = attribute,
                BaseValue = baseValue,
                Modifiers = SetModifiers(attribute)
            };
        
        public int GetCurrentAttributeValue(UnitAttribute attribute) =>
            Attributes.Find(a => a.UnitAttribute == attribute).CurrentValue;

        private List<AttributeModifier> SetModifiers(UnitAttribute attribute) {
            var result = new List<AttributeModifier>();
            result.AddRange(Faction.GetAttributeModifiersByAttribute(attribute));
            if (FighterClass > FighterClass.Juve) {
                if (attribute == UnitAttribute.Aim) {
                    result.Add(new AttributeModifier {
                        Attribute = UnitAttribute.Aim,
                        Modifier = 1
                    });
                }
                if (attribute == UnitAttribute.CloseQuarters) {
                    result.Add(new AttributeModifier {
                        Attribute = UnitAttribute.CloseQuarters,
                        Modifier = 1
                    });
                }
                if (attribute == UnitAttribute.HitPoints) {
                    result.Add(new AttributeModifier {
                        Attribute = UnitAttribute.HitPoints,
                        Modifier = 10
                    });
                }
            }
            
            if (FighterClass > FighterClass.Ganger) {
                if (attribute == UnitAttribute.ActionPoints) {
                    result.Add(new AttributeModifier {
                        Attribute = UnitAttribute.ActionPoints,
                        Modifier = 1
                    });
                }

                if (attribute == UnitAttribute.Movement) {
                    result.Add(new AttributeModifier {
                        Attribute = UnitAttribute.Movement,
                        Modifier = -1
                    });
                }
            }

            if (FighterClass > FighterClass.Veteran) {
                
            }
            
            return result;
        }
    }
    
    public class Attribute {
        public UnitAttribute UnitAttribute { get; set; }
        public int BaseValue { get; set; }
        public List<AttributeModifier> Modifiers { get; set; }
        public int CurrentValue => BaseValue + Modifiers.Sum(m => m.Modifier);
    }
    
    public class AttributeModifier {
        public int Modifier { get; set; }
        public UnitAttribute Attribute { get; set; }
    }
    
    public enum UnitAttribute {
        Movement = 0,
        Aim = 1,
        CloseQuarters = 2,
        HitPoints = 3,
        ActionPoints = 4,
    }
    
    public enum FighterClass {
        Juve = 0,
        Ganger = 1,
        Veteran = 2,
        Champion = 3
    }
}