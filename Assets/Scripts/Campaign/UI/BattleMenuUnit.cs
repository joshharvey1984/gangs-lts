using Gangs.Data;
using TMPro;
using UnityEngine;

namespace Gangs.Campaign.UI
{
    public class BattleMenuUnit : MonoBehaviour {
        [SerializeField] private GameObject unitNameText;
        
        public void SetUnit(CampaignUnit unit) {
            unitNameText.GetComponent<TMP_Text>().text = unit.BaseUnit.Name;
        }
        
        public void SetUnit(Monster monster) {
            unitNameText.GetComponent<TMP_Text>().text = monster.Name;
        }
    }
}
