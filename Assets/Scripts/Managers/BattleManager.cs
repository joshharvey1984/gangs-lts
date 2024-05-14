using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Battle;
using Gangs.Battle.AI;
using Gangs.Core;
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
            SetPlayerSquad();
        }
        
        public BattleUnit GetSelectedUnit() => _battle.BattleBase.ActiveSquad.SelectedUnit;
        public Ability CurrentAbility() => GetSelectedUnit().SelectedAbility;
        public void StartBattle() => _battle.StartBattle();
        public void NextUnit() => _battle.BattleBase.ActiveSquad.NextUnit();
        public void EndUnitTurn() => _battle.BattleBase.ActiveSquad.EndUnitTurn();
        public CoverType GetCoverType(GridPosition gridPosition, CardinalDirection direction) => 
            _battle.BattleBase.GetCoverType(gridPosition, direction);
        public List<BattleUnit> GetActiveEnemyUnits(BattleUnit battleUnit) => _battle.BattleBase.GetActiveEnemyUnits(battleUnit).ToList();
        
        public void MoveUnit(BattleUnit battleUnit, GridPosition gridPosition) {
            var tile = _battle.BattleBase.Grid.Grid.GetTile(gridPosition);
            _battle.MoveUnit(battleUnit, tile);
        }
        
        private void CheckEndTurn() {
            if (GetSelectedUnit().SelectedAbility.EndTurnOnUse || GetSelectedUnit().ActionPointsRemaining <= 0) {
                EndUnitTurn();
                return;
            }
            
            StartPlayerTurn();
        }
        
        private void StartPlayerTurn() {
            BattleInputManager.Instance.SetPlayerControl();
            BattleUIManager.Instance.ShowAbilityButtonBar(GetSelectedUnit());
            GetSelectedUnit().SelectedAbility.Select();
        }
        
        private void StartAITurn() {
            BattleUIManager.Instance.HideAbilityButtonBar();
            BattleAI.TakeTurn();
        }
        
        private void SetPlayerSquad() {
            BattleStartManager.Instance.BattleData.BattleSquadData.ForEach(s => {
                if (s.PlayerControlled) s.Squad.OnUnitStartTurn += StartPlayerTurn;
                else s.Squad.OnUnitStartTurn += StartAITurn;
            });
            
            foreach (var u in _battle.GetUnits().Where(u => BattleStartManager.Instance.BattleData.PlayerControllerUnit(u))) {
                foreach (var ability in u.Abilities) {
                    ability.OnAbilitySelected += BattleGridVisualManager.Instance.DrawTargetingTiles;
                    ability.OnAbilityExecuted += _ => BattleGridVisualManager.Instance.ResetAllVisuals();
                    ability.OnAbilityExecuted += _ => BattleInputManager.Instance.SetPlayerControl(false);
                    ability.OnAbilityExecuted += _ => BattleUIManager.Instance.DisableAbilityButtons();
                    BattleInputManager.Instance.OnLeftClickTile += ability.SetTarget;
                    ability.OnAbilityFinished += CheckEndTurn;
                }
            }
        }

        public GameObject GetTileGameObject(GridPosition tileGridPosition) {
            var tile = _battle.BattleBase.Grid.Grid.GetTile(tileGridPosition);
            return GameObject.FindGameObjectsWithTag("Tile").FirstOrDefault(t => t.transform.position == tile.GridPosition.ToVector3());
        }
    }
}