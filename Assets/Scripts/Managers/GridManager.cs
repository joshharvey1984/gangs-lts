using Gangs.Grid;
using UnityEngine;

namespace Gangs.Managers {
    public class GridManager : MonoBehaviour {
        public static GridManager Instance;
        public Grid.Grid Grid { get; private set; }
        
        public void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
        }
        
        public void RemoveGridUnit(GridUnit gridUnit) => Grid.RemoveUnit(gridUnit);
    }
}