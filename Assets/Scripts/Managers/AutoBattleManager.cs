using Gangs.Abilities.Structs;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class AutoBattleManager : MonoBehaviour {
        public static AutoBattleManager Instance { get; private set; }
        
        private IBattle _battle;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        public void StartBattle(IBattle battle) {
            _battle = battle;
            BattleAI.BattleBase = _battle.BattleBase;
            _battle.BattleBase.Squads.ForEach(s => s.OnUnitStartTurn += BattleAI.TakeTurn);
            _battle.GetUnits().ForEach(u => u.OnMoveUnitTile += MoveUnit);
            _battle.StartBattle();
        }
        
        private void MoveUnit(BattleUnit unit, MoveWaypoint waypoint) {
            waypoint.Tiles.ForEach(t => _battle.MoveUnit(unit, t));
            unit.MoveNextWaypointTile();
        }
    }
}
