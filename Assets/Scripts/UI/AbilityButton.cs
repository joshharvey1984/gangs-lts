using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gangs.UI {
    public class AbilityButton : MonoBehaviour {
        private KeyCode _key;
        public TextMeshProUGUI abilityNameText;
        public TextMeshProUGUI hotkeyText;
        
        private Button _button;
        
        public event Action OnSelected;
        
        private void Awake() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Execute);
        }

        private void Update() {
            if (Input.GetKeyDown(_key)) {
                Execute();
            }
        }

        public void SetAbility(string ability, KeyCode hotkey) {
            _key = hotkey;
            abilityNameText.text = ability;
            hotkeyText.text = hotkey.ToString().Replace("Alpha", "");
        }
        
        public void Enabled(bool enable) {
            _button.interactable = enable;
        }

        private void Execute() {
            OnSelected?.Invoke();
        }
    }
}