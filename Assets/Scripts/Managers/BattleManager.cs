using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.AI;
using Gangs.Battle;
using Gangs.GameObjects;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;
using Wall = Gangs.Grid.Wall;

namespace Gangs.Managers {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        
        public List<BattleSquad> Squads { get; private set; }
        public BattleSquad BattleSquadTurn;
        
        public GameObject soldierPrefab;
        public GameObject selectionCirclePrefab;
        public GameObject abilityUIPanel;
        
        private Grid.Grid Grid => GridManager.Instance.Grid;
        
        public BattleUnit SelectedBattleUnit => BattleSquadTurn.SelectedBattleUnit;
        public bool ActivatedUnit => BattleSquadTurn.ActivatedUnit;

        public bool SquadOnePlayerControlled;
        public bool SquadTwoPlayerControlled;
        
        public EnemyAIWeightings SquadOneWeightings;
        public EnemyAIWeightings SquadTwoWeightings;
        
        public int roundNumber = 1;
        
        public int squadOneScore = 0;
        public int squadTwoScore = 0;
        
        public float totalGameLength = 0;
        public float averageGameLength = 0;
        
        public float globalMoveSpeed = 2f;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            //DataManager.CreateData();
        }
        
        private void Start() {
            GridManager.Instance.SetupGrid();
            LineOfSight.BuildLineOfSightData(GridManager.Instance.Grid);
            
            SpawnSquads();
            BattleSquadTurn = Squads[0];
            NextUnit();
        }
        
        private void Update() {
            //totalGameLength += Time.deltaTime;
            //averageGameLength = totalGameLength / (squadOneScore + squadTwoScore + 1);
        }
        
        private void SpawnSquads() {
            var spawnPositions1 = new List<GridPosition> {
                new(0, 0, 0),
                new(1, 0, 0)
            };
            
            var spawnPositions2 = new List<GridPosition> {
                new(2, 0, 12),
                new(3, 0, 13)
            };
            
            Squads = new List<BattleSquad>();
            
            //CreateSquad(Gang.All[0], spawnPositions1, SquadOneWeightings, Color.blue, SquadOnePlayerControlled);
            //CreateSquad(Gang.All[1], spawnPositions2, SquadTwoWeightings, Color.red, SquadTwoPlayerControlled);
        }
        
        // private void CreateSquad(Gang gang, IList<GridPosition> spawnPositions, EnemyAIWeightings aiWeightings = default, Color color = default, bool playerControlled = false) {
        //     var squad = playerControlled ? (BattleSquad) new PlayerBattleSquad() : new AIBattleSquad(aiWeightings);
        //     Squads.Add(squad);
        //     
        //     foreach (var fighter in gang.Fighters) {
        //         var position = spawnPositions[0];
        //         spawnPositions.RemoveAt(0);
        //         squad.Units.Add(CreateUnit(fighter, position,playerControlled, color));
        //     }
        // }
        
        // private BattleUnit CreateUnit(Unit unit, GridPosition position, bool isPlayerControlled, Color? color = default) {
        //     var unit = new BattleUnit(fighter) {
        //         UnitGameObject = SpawnUnit(position),
        //         GridUnit = Grid.AddUnit(position),
        //         IsPlayerControlled = isPlayerControlled
        //     };
        //     unit.UnitGameObject.UnitNewPosition += MoveUnit;
        //     unit.OnSelected += abilityUIPanel.GetComponent<AbilityButtonBar>().ShowAbilityButtons;
        //     unit.UnitGameObject.modelObject.GetComponent<Renderer>().material.color = color ?? Color.white;
        //     return unit;
        // }
        
        private UnitGameObject SpawnUnit(GridPosition pos) {
            var soldier = Instantiate(soldierPrefab, new Vector3(pos.X, pos.Y, pos.Z), Quaternion.identity);
            soldier.GetComponent<UnitGameObject>().selectionCircleObject = Instantiate(selectionCirclePrefab, soldier.transform);
            return soldier.GetComponent<UnitGameObject>();
        }
        
        private void MoveUnit(GridPosition gridPosition) {
            var movingUnit = BattleSquadTurn.SelectedBattleUnit;
            Grid.MoveUnit(movingUnit.GridUnit, gridPosition);
            
            var movedUnitTile = Grid.GetTile(gridPosition);
            var enemySquads = Squads.Where(s => s != BattleSquadTurn).ToList();
            var enemyUnits = enemySquads.SelectMany(s => s.Units).Where(u => u.Status != Status.Eliminated).ToList();

            foreach (var enemyUnit in enemyUnits) {
                var enemyTile = GetSoldierTile(enemyUnit);
                var enemySquad = enemySquads.FirstOrDefault(s => s.Units.Contains(enemyUnit));
                if (enemyTile.LineOfSightGridPositions.Contains(movedUnitTile.GridPosition)) {
                    BattleSquadTurn.AddOrUpdateEnemyLastSeen(enemyUnit, enemyTile);
                    enemySquad!.AddOrUpdateEnemyLastSeen(movingUnit, movedUnitTile);
                }
            }
        }

        public Tile GetSoldierTile(BattleUnit battleUnit) => Grid.GetTile(GetSoldierPosition(battleUnit.UnitGameObject));
        private GridPosition GetSoldierPosition(UnitGameObject unitGameObject) => new(unitGameObject.Position.x, unitGameObject.Position.y, unitGameObject.Position.z);
        public void NextUnit() => BattleSquadTurn.NextUnit();
        public BattleUnit FindUnit(GridUnit hoverTileGridUnit) => BattleSquadTurn.Units.FirstOrDefault(unit => unit.GridUnit == hoverTileGridUnit);
        public GameObject GetTileGameObject(GridPosition position) => GameObject.FindGameObjectsWithTag("Tile").
            FirstOrDefault(tile => tile.transform.position == new Vector3(position.X, position.Y, position.Z));
        public GameObject GetWallGameObject(Wall wall) => 
            GameObject.FindGameObjectsWithTag("Wall").FirstOrDefault(wallGameObject => 
                wallGameObject.GetComponent<WallGameObject>().Wall == wall);

        public void EndSquadTurn() {
            if (Squads.All(s => s.AllUnitsTurnTaken)) EndRound(); 

            BattleSquadTurn = GetNextSquadTurn();
            BattleSquadTurn.NextUnit();
        }
        
        private BattleSquad GetNextSquadTurn(BattleSquad battleSquad = null) {
            battleSquad ??= BattleSquadTurn;
            var nextSquadIndex = Squads.IndexOf(battleSquad) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken ? GetNextSquadTurn(Squads[nextSquadIndex]) : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Squads.ForEach(s => s.ResetTurns());
            roundNumber++;
            
            if (roundNumber > 100) {
                EndGame();
            }
        }

        public void CheckForEndGame() {
            if (Squads.Any(s => s.Units.All(u => u.Status == Status.Eliminated))) {
                if (Squads[0].Units.All(u => u.Status == Status.Eliminated)) {
                    squadTwoScore++;
                }
                else {
                    squadOneScore++;
                }
                Debug.Log($"Squad 1: {squadOneScore} - Squad 2: {squadTwoScore}");
                throw new EndGameException();
            }
        }
        
        public void EndGame() {
            Debug.Log("Game Over");
            GridVisualManager.Instance.ResetAllVisuals();
            DestroySquads();
            roundNumber = 1;
            SpawnSquads();
            BattleSquadTurn = Squads[0];
            NextUnit();
        }

        private void DestroySquads() {
            Squads.ForEach(s => {
                foreach (var unit in s.Units) {
                    if (GridManager.Instance.Grid.FindGridUnit(unit.GridUnit) != null) {
                        GridManager.Instance.RemoveGridUnit(unit.GridUnit);
                    }

                    if (unit.UnitGameObject != null) {
                        Destroy(unit.UnitGameObject.gameObject);
                    }
                }
            });
            Squads.Clear();
        }
    }
    
    public class EndGameException : Exception {
        public EndGameException() : base("Game Over") { }
    }
}
