using System.Collections.Generic;
using System.Linq;
using Gangs.Battle;
using Gangs.Battle.GameObjects;
using Gangs.Battle.Grid;
using Gangs.Core;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class BattleGridManager : MonoBehaviour {
        public static BattleGridManager Instance { get; private set; }
        
        [SerializeField] private GameObject gridParent;
        
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject halfWallPrefab;
        [SerializeField] private GameObject ladderPrefab;
        
        [SerializeField] private GameObject unitPrefab;

        private BattleGrid _grid;
        private List<UnitGameObject> _unitGameObjects = new();
        
        private void Awake() {
            if (Instance != null && Instance != this) Destroy(this); 
            else Instance = this;

            var battleGrid = BattleStartManager.Instance.Battle.BattleBase.Grid;
            CreateGrid(battleGrid);
            
            var gridUnits = BattleStartManager.Instance.Battle.GetUnits();
            foreach (var gridUnit in gridUnits) {
                _unitGameObjects.Add(SpawnUnit(gridUnit));
            }
            
            BattleManager.Instance.StartBattle(_unitGameObjects);
        }
        
        private UnitGameObject SpawnUnit(BattleUnit unit) {
            var spawnPos = unit.GridUnit.GetTile().GridPosition.ToVector3();
            var unitObject = Instantiate(unitPrefab, spawnPos, Quaternion.identity, gridParent.transform);
            var unitGameObject = unitObject.GetComponent<UnitGameObject>();
            unitGameObject.SetBattleUnit(unit);
            return unitGameObject;
        }

        private void CreateGrid(BattleGrid grid) {
            _grid = grid;

            for (var i = 0; i < _grid.Grid.Tiles.GetLength(0); i++) {
                for (var j = 0; j < _grid.Grid.Tiles.GetLength(1); j++) {
                    for (var k = 0; k < _grid.Grid.Tiles.GetLength(2); k++) {
                        var tile = _grid.Grid.GetTile(i, j, k);
                        if (tile is not null) SpawnTile(tile);
                    }
                }
            }
        }
        
        private void SpawnTile(Tile tile) {
            Instantiate(tilePrefab, tile.GridPosition.ToVector3(), Quaternion.identity, gridParent.transform);
                        
            foreach (var wall in tile.Walls.Where(w => w.Key is CardinalDirection.North or CardinalDirection.East)) {
                var prefab = _grid.GetCoverType(tile.GridPosition, wall.Key) == CoverType.Full ? wallPrefab : halfWallPrefab; 
                var wallObject = Instantiate(prefab, tile.GridPosition.ToVector3(), Quaternion.identity, gridParent.transform);
                if (wall.Key == CardinalDirection.East) {
                    wallObject.transform.Rotate(0, 90, 0);
                    wallObject.transform.position += new Vector3(0.5f, 0, 0);
                }
                if (wall.Key == CardinalDirection.North) {
                    wallObject.transform.position += new Vector3(0, 0, 0.5f);
                }
            }

            if (tile.Climbable is null) return;
            if (tile.Climbable.LowerTile == tile) {
                Instantiate(ladderPrefab, tile.GridPosition.ToVector3(), Quaternion.identity, gridParent.transform);
            }
        }
    }
}