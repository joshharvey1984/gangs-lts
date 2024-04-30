using System.Collections.Generic;
using Gangs.Data;

namespace Gangs.Campaign {
    public class CampaignGang {
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public List<CampaignUnit> Units { get; set; } = new();
    }
}