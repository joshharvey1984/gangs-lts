using Gangs.Battle;
using Gangs.Battle.AI;
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
            _battle = BattleStartManager.Instance.BattleData.Battle;
            BattleStartManager.Instance.BattleData.BattleSquadData.ForEach(s => {
                if (s.PlayerControlled) s.Squad.OnUnitStartTurn += StartPlayerTurn;
                else s.Squad.OnUnitStartTurn += BattleAI.TakeTurn;
            });
        }
        
        public void StartBattle() {
            _battle.StartBattle();
        }

        public void MoveUnit(BattleUnit battleUnit, GridPosition gridPosition) {
            var tile = _battle.BattleBase.Grid.Grid.GetTile(gridPosition);
            _battle.MoveUnit(battleUnit, tile);
        }

        private void StartPlayerTurn() {
            var selectedUnit = GetSelectedUnit();
            selectedUnit.Abilities[0].Select();
        }
        
        private BattleUnit GetSelectedUnit() => _battle.BattleBase.ActiveSquad.SelectedUnit;
    }
}