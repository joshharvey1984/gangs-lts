using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gangs.UI {
    public class SelectionCircle : MonoBehaviour {
        public State currentState = State.Available;
        
        private Material _material;
        
        private void Awake() {
            _material = GetComponent<MeshRenderer>().material;
        }
        
        private void Update() {
            if (currentState == State.Selected) { transform.Rotate(Vector3.forward * (Time.deltaTime * 75)); }
        }
        
        public void SetState(State state) {
            currentState = state;
            StateColour();
        }
        
        private void StateColour() {
            switch (currentState) {
                case State.Available:
                    _material.color = Color.white;
                    break;
                case State.Unavailable:
                    _material.color = Color.gray;
                    break;
                case State.Selected:
                    _material.color = Color.cyan;
                    break;
            }
        }
        
        [Serializable]
        public enum State {
            Available,
            Unavailable,
            Selected
        }
    }
}
