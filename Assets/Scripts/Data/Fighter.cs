using System.Collections.Generic;
using System.Linq;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Fighter: Entity {
        public static List<Fighter> All { get; set; } = new();
        
        public Clan Clan { get; set; }
        public int Level { get; set; }
        public FighterClass FighterClass { get; set; }
        public List<Attribute> Attributes { get; set; }
        
        public Fighter(FighterDto dto) : base(dto) { 
            All.Add(this);
        }
        
        public void Create(FighterDto dto) {
            Clan = Clan.All.Find(c => c.ID == dto.clanId);
            Level = dto.level;
            FighterClass = Level switch {
                < 6 => FighterClass.Juve,
                < 11 => FighterClass.Ganger,
                < 16 => FighterClass.Veteran,
                _ => FighterClass.Champion
            };

            Attributes = new List<Attribute> {
                SetAttribute(FighterAttribute.Movement, 5),
                SetAttribute(FighterAttribute.Aim, 2),
                SetAttribute(FighterAttribute.CloseQuarters, 2),
                SetAttribute(FighterAttribute.HitPoints, 5),
                SetAttribute(FighterAttribute.ActionPoints, 2)
            };
        }

        protected Attribute SetAttribute(FighterAttribute attribute, int baseValue) =>
            new() {
                FighterAttribute = attribute,
                BaseValue = baseValue,
                Modifiers = SetModifiers(attribute)
            };
        
        public int GetCurrentAttributeValue(FighterAttribute attribute) =>
            Attributes.Find(a => a.FighterAttribute == attribute).CurrentValue;

        private List<AttributeModifier> SetModifiers(FighterAttribute attribute) {
            var result = new List<AttributeModifier>();
            result.AddRange(Clan.GetAttributeModifiersByAttribute(attribute));
            if (FighterClass > FighterClass.Juve) {
                if (attribute == FighterAttribute.Aim) {
                    result.Add(new AttributeModifier {
                        Attribute = FighterAttribute.Aim,
                        Modifier = 1
                    });
                }
                if (attribute == FighterAttribute.CloseQuarters) {
                    result.Add(new AttributeModifier {
                        Attribute = FighterAttribute.CloseQuarters,
                        Modifier = 1
                    });
                }
                if (attribute == FighterAttribute.HitPoints) {
                    result.Add(new AttributeModifier {
                        Attribute = FighterAttribute.HitPoints,
                        Modifier = 10
                    });
                }
            }
            
            if (FighterClass > FighterClass.Ganger) {
                if (attribute == FighterAttribute.ActionPoints) {
                    result.Add(new AttributeModifier {
                        Attribute = FighterAttribute.ActionPoints,
                        Modifier = 1
                    });
                }

                if (attribute == FighterAttribute.Movement) {
                    result.Add(new AttributeModifier {
                        Attribute = FighterAttribute.Movement,
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
        public FighterAttribute FighterAttribute { get; set; }
        public int BaseValue { get; set; }
        public List<AttributeModifier> Modifiers { get; set; }
        public int CurrentValue => BaseValue + Modifiers.Sum(m => m.Modifier);
    }
    
    public class AttributeModifier {
        public int Modifier { get; set; }
        public FighterAttribute Attribute { get; set; }
    }
    
    public enum FighterAttribute {
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