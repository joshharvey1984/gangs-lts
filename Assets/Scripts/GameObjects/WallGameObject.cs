using Gangs.Grid;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gangs.GameObjects {
    public class WallGameObject : MonoBehaviour {
        public Wall Wall { get; set; }

        [SerializeField]
        private CoverType coverType;
        public CoverType CoverType {
            get => coverType;
            set => coverType = value;
        }
    }
    
    public enum CoverType {
        None,
        Half,
        Full
    }
}