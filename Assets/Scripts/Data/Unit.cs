using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Unit : Entity {
        public static List<Unit> All { get; set; } = new();
        
        public List<BaseAttribute> BaseAttributes { get; set; }
        public List<Modifiers> Modifiers { get; set; }
        
        public Unit(UnitDto dto) : base(dto) { 
            All.Add(this);
        }
        
        public void Create(UnitDto dto) {
            SetAttributes(dto);
            SetModifiers(dto);
        }
        
        private UnitAttribute GetAttribute(string attribute) => 
            (UnitAttribute) Enum.Parse(typeof(UnitAttribute), attribute, true);
        private ModifierType GetModifierType(string type) => 
            (ModifierType) Enum.Parse(typeof(ModifierType), type, true);
        
        private void SetModifiers(UnitDto dto) {
            Modifiers = new List<Modifiers>();
            foreach (var modifier in dto.modifiers) {
                var type = GetModifierType(modifier.type);
                switch (type) {
                    case ModifierType.NameChange:
                        Modifiers.Add(new Modifiers {
                            Level = modifier.level,
                            Type = type,
                            Value = modifier.value
                        });
                        break;
                    case ModifierType.AttributeChange:
                        var attribute = GetAttribute(modifier.attribute);
                        Modifiers.Add(new Modifiers {
                            Level = modifier.level,
                            Type = type,
                            Value = new AttributeModifier {
                                Attribute = attribute,
                                Modifier = int.Parse(modifier.value)
                            }
                        });
                        break;
                }
            }
        }

        private void SetAttributes(UnitDto dto) {
            BaseAttributes = new List<BaseAttribute>();
            foreach (var baseAttribute in dto.baseAtrributes) {
                var attribute = GetAttribute(baseAttribute.attribute);
                BaseAttributes.Add(new BaseAttribute {
                    UnitAttribute = attribute,
                    BaseValue = baseAttribute.value
                });
            }
            
            // throw data exception if any attribute is missing
            var missingAttributes = 
                Enum.GetValues(typeof(UnitAttribute)).Cast<UnitAttribute>().Except(BaseAttributes.Select(a => a.UnitAttribute));
            var unitAttributes = missingAttributes.ToList();
            if (unitAttributes.Any()) {
                throw new DataException($"Missing attributes: {string.Join(", ", unitAttributes)}");
            }
            
            // throw data exception if any attribute is duplicated
            var duplicatedAttributes = 
                BaseAttributes.GroupBy(a => a.UnitAttribute).Where(g => g.Count() > 1).Select(g => g.Key);
            var attributes = duplicatedAttributes.ToList();
            if (attributes.Any()) {
                throw new DataException($"Duplicated attributes: {string.Join(", ", attributes)}");
            }
        }
    }
    
    public class BaseAttribute {
        public UnitAttribute UnitAttribute { get; set; }
        public int BaseValue { get; set; }
    }
    
    public class Modifiers {
        public int Level { get; set; }
        public ModifierType Type { get; set; }
        public object Value { get; set; }
        
        public string GetNameChange() => (string) Value;
        public AttributeModifier GetAttributeChange() => (AttributeModifier) Value;
    }
    
    public class AttributeModifier {
        public UnitAttribute Attribute { get; set; }
        public int Modifier { get; set; }
    }
    
    public enum ModifierType {
        NameChange = 0,
        AttributeChange = 1,
    }
    
    public enum UnitAttribute {
        Movement = 0,
        Aim = 1,
        CloseQuarters = 2,
        HitPoints = 3,
        ActionPoints = 4,
    }
}