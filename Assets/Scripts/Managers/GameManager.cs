using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
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
                new(10, 0, 15),
                new(11, 0, 15)
            };
            
            Squads = new List<Squad>();
            Gang.All[0].IsPlayerControlled = true;
            CreateSquad(Gang.All[0], spawnPositions1);
            CreateSquad(Gang.All[1], spawnPositions2);
        }
        
        private void CreateSquad(Gang gang, IList<GridPosition> spawnPositions) {
            var squad = gang.IsPlayerControlled ? (Squad) new PlayerSquad() : new AISquad();
            Squads.Add(squad);
            
            foreach (var fighter in gang.Fighters) {
                var position = spawnPositions[0];
                spawnPositions.RemoveAt(0);
                squad.Units.Add(CreateUnit(fighter, position));
            }
        }
        
        private Unit CreateUnit(Fighter fighter, GridPosition position) {
            var unit = new Unit(fighter) {
                UnitGameObject = SpawnUnit(position),
                GridUnit = Grid.AddUnit(position)
            };
            unit.UnitGameObject.UnitNewPosition += MoveUnit;
            unit.OnSelected += abilityUIPanel.GetComponent<AbilityButtonBar>().ShowAbilityButtons;
            return unit;
        }
        
        private UnitGameObject SpawnUnit(GridPosition pos) {
            var soldier = Instantiate(soldierPrefab, new Vector3(pos.X, pos.Y, pos.Z), Quaternion.identity);
            soldier.GetComponent<UnitGameObject>().selectionCircleObject = Instantiate(selectionCirclePrefab, soldier.transform);
            return soldier.GetComponent<UnitGameObject>();
        }
        
        private void MoveUnit(GridPosition gridPosition) => Grid.MoveUnit(SquadTurn.SelectedUnit.GridUnit, gridPosition);
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
        
        private Squad GetNextSquadTurn() {
            var nextSquadIndex = Squads.IndexOf(SquadTurn) + 1;
            if (nextSquadIndex >= Squads.Count) nextSquadIndex = 0;
            return Squads[nextSquadIndex].AllUnitsTurnTaken ? GetNextSquadTurn() : Squads[nextSquadIndex];
        }
        
        private void EndRound() {
            Squads.ForEach(s => s.ResetTurns());
        }
    }
}
