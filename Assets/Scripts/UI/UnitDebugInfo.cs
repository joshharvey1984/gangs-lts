using System.Collections.Generic;
using Gangs.Battle;
using UnityEngine;

namespace Gangs.UI
{
    public class UnitDebugInfo : MonoBehaviour {
        private Dictionary<BattleUnit, GameObject> _unitInfo;
        private void Update() {
            UpdateInfo();
        }

        private void Start() {
            _unitInfo = new Dictionary<BattleUnit, GameObject>();
            var squads = Managers.BattleManager.Instance.Squads;
            UnitListInfo(squads);
        }

        public void UnitListInfo(List<BattleSquad> squads) {
            var count = 0;
            foreach (var squad in squads) {
                foreach (var unit in squad.Units) {
                    var TMPGameObject = new GameObject();
                    var textMeshPro = TMPGameObject.AddComponent<TMPro.TextMeshPro>();
                    textMeshPro.transform.SetParent(gameObject.transform);
                    textMeshPro.fontSize = 150;
                    textMeshPro.color = Color.black;
                    textMeshPro.alignment = TMPro.TextAlignmentOptions.Left;
                    textMeshPro.rectTransform.sizeDelta = new Vector2(500, 100);
                    textMeshPro.rectTransform.position = transform.position;
                    textMeshPro.rectTransform.position += new Vector3(0, count * 100, 0);
                    count++;
                    
                    var unitInfo = $"Unit: {unit.Unit.Name}\n" +
                                   $"Action Points: {unit.ActionPointsRemaining}\n" +
                                   $"Turn Taken: {unit.TurnTaken}\n" +
                                   $"Player Controlled: {unit.IsPlayerControlled}\n" +
                                   $"Squad: {squad.GetType().Name}\n";
                    
                    textMeshPro.text = unitInfo;
                    _unitInfo.Add(unit, TMPGameObject);
                }
            }
        }
        
        public void UpdateInfo() {
            foreach (var unit in _unitInfo.Keys) {
                var textMeshPro = _unitInfo[unit].GetComponent<TMPro.TextMeshPro>();
                var unitInfo = $"Unit: {unit.Unit.Name}\n" +
                               $"Action Points: {unit.ActionPointsRemaining}\n" +
                               $"Turn Taken: {unit.TurnTaken}\n" +
                               $"Player Controlled: {unit.IsPlayerControlled}\n" +
                               $"Squad: {unit.GetType().Name}\n";
                textMeshPro.text = unitInfo;
            }
        }
    }
}
