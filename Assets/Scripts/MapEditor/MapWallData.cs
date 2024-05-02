using Gangs.Core;
using UnityEngine;

namespace Gangs.MapEditor {
    public class MapWallData : MonoBehaviour {
        [SerializeField] public CoverType coverType;
        [SerializeField] public bool lineOfSightBlocker;
    }
}