using System.Collections.Generic;
using Gangs.Core;

namespace Gangs.Campaign {
    public class CampaignUnit {
        public string Class { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public List<UnitAttribute> Attributes { private get; set; } = new();
        
        public UnitAttribute GetAttribute(UnitAttributeType type) => Attributes.Find(a => a.Type == type);
    }
}