using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gangs.Campaign
{
    public class CityMap : MonoBehaviour {
        private List<GameObject> _teritories;
        private void Awake() {
            _teritories = new List<GameObject>();
            _teritories.AddRange(gameObject.GetComponentsInChildren<Transform>().Where(t => t != transform).Select(t => t.gameObject));
        }
        
        private void Start() {
            var middle = new Vector3();
            middle = _teritories.Aggregate(middle, (current, tile) => current + tile.transform.position);
            middle /= _teritories.Count;
            
            foreach (var t in _teritories) {
                var distance = Vector3.Distance(middle, t.transform.position);
                t.transform.position += (t.transform.position - middle).normalized * distance * 0.1f;
            }
            
            foreach (var territory in _teritories) {
                territory.GetComponent<TerritoryGameObject>().CreateBorder();
            }
        }
    }
}
