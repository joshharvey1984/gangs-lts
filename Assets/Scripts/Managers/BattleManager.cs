using System.Collections.Generic;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Battle.GameObjects;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        
        private IBattle _battle;
        private List<UnitGameObject> _unitGameObjects;
        
        public float globalMoveSpeed = 2f;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _battle = BattleStartManager.Instance.Battle;
        }
        
        public void StartBattle(List<UnitGameObject> unitGameObjects) {
            _unitGameObjects = unitGameObjects;
            BattleAI.BattleBase = _battle.BattleBase;
            _battle.StartBattle();
        }
    }
}
