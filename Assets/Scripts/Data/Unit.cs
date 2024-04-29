using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Gangs.Core;
using Gangs.Data.DTO;

namespace Gangs.Data {
    public class Unit : Entity {
        public static List<Unit> All { get; set; } = new();
        
        public List<BaseAttribute> BaseAttributes { get; set; }
        public List<Modifier> Modifiers { get; set; }
        
        public Unit(UnitDto dto) : base(dto) { 
            All.Add(this);
        }
        
        public void Create(UnitDto dto) {
            SetAttributes(dto);
            SetModifiers(dto);
        }
        
        public List<Modifier> GetModifiers(int level) => Modifiers.Where(m => m.Level <= level).ToList();
        
        private UnitAttributeType GetAttribute(string attribute) => 
            (UnitAttributeType) Enum.Parse(typeof(UnitAttributeType), attribute, true);
        private ModifierType GetModifierType(string type) => 
            (ModifierType) Enum.Parse(typeof(ModifierType), type, true);
        
        private void SetModifiers(UnitDto dto) {
            Modifiers = new List<Modifier>();
            
            foreach (var modifier in dto.modifiers) {
                var type = GetModifierType(modifier.type);
                if (type == ModifierType.NameChange) {
                    Modifiers.Add(new Modifier {
                        Level = modifier.level,
                        Type = type,
                        Value = modifier.value
                    });
                }
                if (type == ModifierType.AttributeChange) {
                    Modifiers.Add(new Modifier {
                        Level = modifier.level,
                        Type = type,
                        Value = new AttributeModifier {
                            AttributeType = GetAttribute(modifier.attribute),
                            Modifier = int.Parse(modifier.value)
                        }
                    });
                }
            }
        }

        private void SetAttributes(UnitDto dto) {
            BaseAttributes = new List<BaseAttribute>();
            foreach (var baseAttribute in dto.baseAtrributes) {
                var attribute = GetAttribute(baseAttribute.attribute);
                BaseAttributes.Add(new BaseAttribute {
                    UnitAttributeType = attribute,
                    BaseValue = baseAttribute.value
                });
            }
            
            // throw data exception if any attribute is missing
            var missingAttributes = 
                Enum.GetValues(typeof(UnitAttributeType)).Cast<UnitAttributeType>().Except(BaseAttributes.Select(a => a.UnitAttributeType));
            var unitAttributes = missingAttributes.ToList();
            if (unitAttributes.Any()) {
                throw new DataException($"Missing attributes: {string.Join(", ", unitAttributes)}");
            }
            
            // throw data exception if any attribute is duplicated
            var duplicatedAttributes = 
                BaseAttributes.GroupBy(a => a.UnitAttributeType).Where(g => g.Count() > 1).Select(g => g.Key);
            var attributes = duplicatedAttributes.ToList();
            if (attributes.Any()) {
                throw new DataException($"Duplicated attributes: {string.Join(", ", attributes)}");
            }
        }
    }
    
    public class BaseAttribute {
        public UnitAttributeType UnitAttributeType { get; set; }
        public int BaseValue { get; set; }
    }
    
    public class Modifier {
        public int Level { get; set; }
        public ModifierType Type { get; set; }
        public object Value { get; set; }
        
        public string GetNameChange() => (string) Value;
        public AttributeModifier GetAttributeChange() => (AttributeModifier) Value;
    }
    
    public class AttributeModifier {
        public UnitAttributeType AttributeType { get; set; }
        public int Modifier { get; set; }
    }
    
    public enum ModifierType {
        NameChange = 0,
        AttributeChange = 1,
    }
}