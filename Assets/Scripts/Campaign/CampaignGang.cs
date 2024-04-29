using System.Collections.Generic;

namespace Gangs.Campaign {
    public class CampaignGang {
        public string Name { get; set; }
        public List<CampaignUnit> Units { get; set; } = new();
    }
}