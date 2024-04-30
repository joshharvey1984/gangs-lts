using System.Collections.Generic;
using System.Linq;

namespace Gangs.Core {
    public class UnitAttribute {
        public UnitAttributeType Type { get; set; }
        public int BaseValue { get; set; }
        public List<UnitAttributeModifier> Modifiers { get; set; } = new();
        
        public int GetValue() => BaseValue + Modifiers.Sum(modifier => modifier.Value);
    }
    
    public enum UnitAttributeType {
        Movement = 0,
        Aim = 1,
        CloseQuarters = 2,
        HitPoints = 3,
        ActionPoints = 4,
    }

    public class UnitAttributeModifier {
        public UnitAttributeModifierSource Source { get; set; }
        public int Value { get; set; }
    }
    
    public class UnitAttributeModifierSource {
        public UnitAttributeModifierSourceType Type { get; set; }
        public string Name { get; set; }
    }

    public enum UnitAttributeModifierSourceType {
        Faction,
        Individual,
        Buff
    }
}