using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gangs.UI {
    public class AbilityButtonBar : MonoBehaviour {
        [SerializeField]
        private GameObject abilityButtonPrefab;
        
        private readonly List<AbilityButton> _abilityButtons = new();
        
        public void ShowAbilityButtons(Unit unit) {
            DestroyAbilityButtons();
            foreach (var ability in unit.Abilities) {
                var button = Instantiate(abilityButtonPrefab, gameObject.transform).GetComponent<AbilityButton>();
                var hotkey = unit.Abilities.ToList().IndexOf(ability) + 1;
                var keyCode = (KeyCode) Enum.Parse(typeof(KeyCode), "Alpha" + hotkey);
                button.SetAbility(ability.ButtonText, keyCode);
                button.OnSelected += ability.Select;
                _abilityButtons.Add(button);
            }
        }
        
        public void DisableAbilityButtons() => _abilityButtons.ForEach(b => b.Enabled(false));
        public void EnableAbilityButtons() => _abilityButtons.ForEach(b => b.Enabled(true));
        public void DestroyAbilityButtons() {
            foreach (var button in _abilityButtons) {
                Destroy(button.gameObject);
            }
            _abilityButtons.Clear();
        }
    }
}