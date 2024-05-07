using Gangs.Battle;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        
        private IBattle _battle;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _battle = BattleStartManager.Instance.Battle;
        }
        
        public void StartBattle() {
            _battle.StartBattle();
        }

        public void MoveUnit(BattleUnit battleUnit, GridPosition gridPosition) {
            var tile = _battle.BattleBase.Grid.Grid.GetTile(gridPosition);
            _battle.MoveUnit(battleUnit, tile);
        }
    }
}