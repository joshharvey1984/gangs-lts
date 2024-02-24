using System;
using Gangs.Abilities;
using UnityEngine;

namespace Gangs {
    public class AbilityButton : MonoBehaviour { 
        public BaseAbility Ability { get; set; }
        
        public event Action<BaseAbility> OnSelected;
        
        public void Execute() {
            OnSelected?.Invoke(Ability);
            Ability.Execute();
        }
    }
}