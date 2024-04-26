namespace Gangs.Campaign {
    public interface ICampaignEntity {
        public CampaignEntityGameObject GameObject { get; set; }
        string Name { get; set; }
    }
}