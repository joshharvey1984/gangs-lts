using UnityEngine;

namespace Gangs.Campaign.UI {
    public class TerritoryInfoPanel : MonoBehaviour {
        [SerializeField] private GameObject typeText;
        [SerializeField] private GameObject gangText;
        [SerializeField] private GameObject clanText;

        public void SetTerritory(CampaignTerritory territory) {
            typeText.GetComponent<TMPro.TextMeshProUGUI>().text = territory.Territory.Name;
            gangText.GetComponent<TMPro.TextMeshProUGUI>().text = territory.ClaimedBy?.Name ?? "Unclaimed";
            clanText.GetComponent<TMPro.TextMeshProUGUI>().text = territory.ClaimedBy?.Clan.Name ?? "";
            
            gangText.GetComponent<TMPro.TextMeshProUGUI>().color = territory.ClaimedBy?.Clan.Color ?? Color.white;
            clanText.GetComponent<TMPro.TextMeshProUGUI>().color = territory.ClaimedBy?.Clan.Color ?? Color.white;
        }
    }
}