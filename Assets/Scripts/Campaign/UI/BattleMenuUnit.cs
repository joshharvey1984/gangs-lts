using TMPro;
using UnityEngine;

namespace Gangs.Campaign.UI
{
    public class BattleMenuUnit : MonoBehaviour {
        [SerializeField] private GameObject unitNameText;
        
        public void SetUnit(CampaignUnit unit) {
            unitNameText.GetComponent<TMP_Text>().text = unit.Name;
        }
    }
}
