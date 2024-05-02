using System;
using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.GameObjects;
using Gangs.Grid;
using UnityEngine;
using Tile = Gangs.Grid.Tile;
using Wall = Gangs.Grid.Wall;

namespace Gangs.Managers {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance { get; private set; }
        
        public GameObject soldierPrefab;
        public GameObject selectionCirclePrefab;
        public GameObject abilityUIPanel;
        
        private Battle.Battle _battle;
        
        // public int squadOneScore = 0;
        // public int squadTwoScore = 0;
        //
        // public float totalGameLength = 0;
        // public float averageGameLength = 0;
        
        public float globalMoveSpeed = 2f;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        private void Start() {
            //SpawnSquads();
        }
        
        private void Update() {
            //totalGameLength += Time.deltaTime;
            //averageGameLength = totalGameLength / (squadOneScore + squadTwoScore + 1);
        }
        
        // private void SpawnSquads() {
        //     //CreateSquad(Gang.All[0], spawnPositions1, SquadOneWeightings, Color.blue, SquadOnePlayerControlled);
        //     //CreateSquad(Gang.All[1], spawnPositions2, SquadTwoWeightings, Color.red, SquadTwoPlayerControlled);
        // }
        
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
        
        // private UnitGameObject SpawnUnit(GridPosition pos) {
        //     var soldier = Instantiate(soldierPrefab, new Vector3(pos.X, pos.Y, pos.Z), Quaternion.identity);
        //     soldier.GetComponent<UnitGameObject>().selectionCircleObject = Instantiate(selectionCirclePrefab, soldier.transform);
        //     return soldier.GetComponent<UnitGameObject>();
        // }
        
        
        // public GameObject GetTileGameObject(GridPosition position) => GameObject.FindGameObjectsWithTag("Tile").
        //     FirstOrDefault(tile => tile.transform.position == new Vector3(position.X, position.Y, position.Z));
        // public GameObject GetWallGameObject(Wall wall) => 
        //     GameObject.FindGameObjectsWithTag("Wall").FirstOrDefault(wallGameObject => 
        //         wallGameObject.GetComponent<WallGameObject>().Wall == wall);

        
        
        
        
        

        
        
        

        // private void DestroySquads() {
        //     Squads.ForEach(s => {
        //         foreach (var unit in s.Units) {
        //             if (GridManager.Instance.Grid.FindGridUnit(unit.GridUnit) != null) {
        //                 GridManager.Instance.RemoveGridUnit(unit.GridUnit);
        //             }
        //
        //             if (unit.UnitGameObject != null) {
        //                 Destroy(unit.UnitGameObject.gameObject);
        //             }
        //         }
        //     });
        //     Squads.Clear();
        // }
    }
    
    
}
