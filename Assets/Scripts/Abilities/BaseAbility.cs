using System;
using Gangs.Grid;
using Gangs.Managers;

namespace Gangs.Abilities {
    public abstract class BaseAbility {
        public string ButtonText;
        public InputMode InputMode;
        public TargetingType TargetingType;
        public bool EndTurnOnUse = false;
        
        public event Action OnFinished;

        public void Execute() => InputManager.Instance.SetAbility(this);
        public virtual void Use(Tile tile) { OnFinished?.Invoke(); }
        public virtual int ToHit(Tile tile) {
            return 0;
        }
        protected void Finish() => OnFinished?.Invoke();
    }
}