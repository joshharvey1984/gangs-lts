using Gangs.Grid;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gangs.GameObjects {
    public class PropGameObject : MonoBehaviour {
        public Prop Prop { get; set; }

        [FormerlySerializedAs("wallCoverType")] [SerializeField]
        private CoverType coverType;
        public CoverType CoverType {
            get => coverType;
            set => coverType = value;
        }
    }
}