using Gangs.Data;
using UnityEngine;

namespace Gangs.Managers {
    public class DebugManager : MonoBehaviour {
        public static DebugManager Instance { get; private set; }
        
        public bool DebugMode;
        
        public bool lineOfSite;

        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void ToggleDebugMode() {
            DebugMode = !DebugMode;
            
            // InputManager.Instance.DrawMoveRanges();
            // InputManager.Instance.DrawMovePath();
            //InputManager.Instance.GetLineOfSight();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F1)) {
                ToggleDebugMode();
            }
        }
    }
}
