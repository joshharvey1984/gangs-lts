using System.Collections.Generic;

namespace Gangs.Campaign {
    public interface ICampaignEntity {
        public CampaignEntityGameObject GameObject { get; set; }
        public string Name { get; set; }
    }
}