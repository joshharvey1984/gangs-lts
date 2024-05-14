using Gangs.Battle;
using Gangs.Battle.UI;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleUIManager : MonoBehaviour {
        public static BattleUIManager Instance { get; private set; }
        
        [SerializeField] private GameObject abilityButtonBar;

        private void Awake() {
            if (Instance is not null && Instance != this) Destroy(this);
            else Instance = this;
        }
        
        public void ShowAbilityButtonBar(BattleUnit unit) {
            abilityButtonBar.GetComponent<AbilityButtonBar>().ShowAbilityButtons(unit);
            ShowAbilityButtonBar();
            EnableAbilityButtons();
        }
        
        public void DisableAbilityButtons() {
            abilityButtonBar.GetComponent<AbilityButtonBar>().DisableAbilityButtons();
        }
        
        public void EnableAbilityButtons() {
            abilityButtonBar.GetComponent<AbilityButtonBar>().EnableAbilityButtons();
        }
        
        public void DestroyAbilityButtons() {
            abilityButtonBar.GetComponent<AbilityButtonBar>().DestroyAbilityButtons();
        }
        
        public void ShowAbilityButtonBar() {
            abilityButtonBar.SetActive(true);
        }
        
        public void HideAbilityButtonBar() {
            DestroyAbilityButtons();
            abilityButtonBar.SetActive(false);
        }
    }
}