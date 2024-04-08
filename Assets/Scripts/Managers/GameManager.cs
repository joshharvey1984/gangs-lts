using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.AI;
using Gangs.Data;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.UI;
using UnityEngine;

namespace Gangs.Managers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }
        
        public List<Squad> Squads { get; private set; }
        public Squad SquadTurn;
        
        public GameObject soldierPrefab;
        public GameObject selectionCirclePrefab;
        public GameObject abilityUIPanel;
        
        private Grid.Grid Grid => GridManager.Instance.Grid;
        
        public Unit SelectedUnit => SquadTurn.SelectedUnit;
        public bool ActivatedUnit => SquadTurn.ActivatedUnit;

        public bool SquadOnePlayerControlled;
        public bool SquadTwoPlayerControlled;
        
        public EnemyAIWeightings SquadOneWeightings;
        public EnemyAIWeightings SquadTwoWeightings;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            DataManager.CreateData();
        }
        
        private void Start() {
            GridManager.Instance.SetupGrid();
            LineOfSight.BuildLineOfSightData();
            
            SpawnSquads();
            SquadTurn = Squads[0];
            NextUnit();
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
            
            Squads = new List<Squad>();
            Gang.All[0].IsPlayerControlled = SquadOnePlayerControlled;
            Gang.All[1].IsPlayerControlled = SquadTwoPlayerControlled;
            CreateSquad(Gang.All[0], spawnPositions1, SquadOneWeightings, Color.blue);
            CreateSquad(Gang.All[1], spawnPositions2, SquadTwoWeightings, Color.red);
        }
        
        private void CreateSquad(Gang gang, IList<GridPosition> spawnPositions, EnemyAIWeightings aiWeightings = default, Color color = default) {
            var squad = gang.IsPlayerControlled ? (Squad) new PlayerSquad() : new AISquad(aiWeightings);
            Squads.Add(squad);
            
            foreach (var fighter in gang.Fighters) {
                var position = spawnPositions[0];
                spawnPositions.RemoveAt(0);
                squad.Units.Add(CreateUnit(fighter, position, gang.IsPlayerControlled, color));
            }
        }
        
        private Unit CreateUnit(Fighter fighter, GridPosition position, bool isPlayerControlled, Color? color = default) {
            var unit = new Unit(fighter) {
                UnitGameObject = SpawnUnit(position),
                GridUnit = Grid.AddUnit(position),
                IsPlayerControlled = isPlayerControlled
            };
            unit.UnitGameObject.UnitNewPosition += MoveUnit;
            unit.OnSelected += abilityUIPanel.GetComponent<AbilityButtonBar>().ShowAbilityButtons;
            unit.UnitGameObject.modelObject.GetComponent<Renderer>().material.color = color ?? Color.white;
            return unit;
        }
        
        private UnitGameObject SpawnUnit(GridPosition pos) {
            var soldier = Instantiate(soldierPrefab, new Vector3(pos.X, pos.Y, pos.Z), Quaternion.identity);
            soldier.GetComponent<UnitGameObject>().selectionCircleObject = Instantiate(selectionCirclePrefab, soldier.transform);
            return soldier.GetComponent<UnitGameObject>();
        }
        
        private void MoveUnit(GridPosition gridPosition) {
            var movingUnit = SquadTurn.SelectedUnit;
            Grid.MoveUnit(movingUnit.GridUnit, gridPosition);
            
            var movedUnitTile = Grid.GetTile(gridPosition);
            var enemySquads = Squads.Where(s => s != SquadTurn).ToList();
            var enemyUnits = enemySquads.SelectMany(s => s.Units).Where(u => u.Status != Status.Eliminated).ToList();

            foreach (var enemyUnit in enemyUnits) {
                var enemyTile = GetSoldierTile(enemyUnit);
                var enemySquad = enemySquads.FirstOrDefault(s => s.Units.Contains(enemyUnit));
                if (enemyTile.LineOfSightGridPositions.Contains(movedUnitTile.GridPosition)) {
                    SquadTurn.AddOrUpdateEnemyLastSeen(enemyUnit, enemyTile);
                    enemySquad!.AddOrUpdateEnemyLastSeen(movingUnit, movedUnitTile);
                }
            }
        }

        public Tile GetSoldierTile(Unit unit) => Grid.GetTile(GetSoldierPosition(unit.UnitGameObject));
        private GridPosition GetSoldierPosition(UnitGameObject unitGameObject) => new(unitGameObject.Position.x, unitGameObject.Position.y, unitGameObject.Position.z);
        public void NextUnit() => SquadTurn.NextUnit();
        public Unit FindUnit(GridUnit hoverTileGridUnit) => SquadTurn.Units.FirstOrDefault(unit => unit.GridUnit == hoverTileGridUnit);
        public GameObject GetTileGameObject(GridPosition position) => GameObject.FindGameObjectsWithTag("Tile").
            FirstOrDefault(tile => tile.transform.position == new Vector3(position.X, position.Y, position.Z));
        public GameObject GetWallGameObject(Wall wall) => 
            GameObject.FindGameObjectsWithTag("Wall").FirstOrDefault(wallGameObject => 
                wallGameObject.GetComponent<WallGameObject>().Wall == wall);
        
        public GameObject GetPropGameObject(Prop prop) =>
            GameObject.FindGameObjectsWithTag("Prop").FirstOrDefault(propGameObject => 
                propGameObject.GetComponent<PropGameObject>().Prop == prop);

        public void EndSquadTurn() {
            if (Squads.All(s => s.AllUnitsTurnTaken)) EndRound(); 

            SquadTurn = GetNextSquadTurn();
            SquadTurn.NextUnit();
        }
        
        private Squad GetNextSquadTurn(Squad squad = null) {
            squad ??= SquadTurn;
            var nextSquadIndex = Squads.IndexOf(squad) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken ? GetNextSquadTurn(Squads[nextSquadIndex]) : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Squads.ForEach(s => s.ResetTurns());
        }

        public void CheckForEndGame() {
            if (Squads.Any(s => s.Units.All(u => u.Status == Status.Eliminated))) {
                throw new EndGameException();
            }
        }
        
        public void EndGame() {
            Debug.Log("Game Over");
            GridVisualManager.Instance.ResetAllVisuals();
            DestroySquads();
            SpawnSquads();
            SquadTurn = Squads[0];
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
