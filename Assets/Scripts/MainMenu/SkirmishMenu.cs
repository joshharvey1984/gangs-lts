using System.Collections.Generic;
using Gangs.Data;
using Gangs.Managers;
using TMPro;
using UnityEngine;

namespace Gangs.MainMenu {
    public class SkirmishMenu : MonoBehaviour {
        // [SerializeField] private GameObject gangDropdown1;
        // private Gang _selectedGang1;
        //
        // [SerializeField] private GameObject gangDropdown2;
        // private Gang _selectedGang2;
        //
        // [SerializeField] private GameObject gangInfoPanel1;
        // [SerializeField] private GameObject gangInfoPanel2;
        //
        // private void OnEnable() {
        //     gangDropdown1.GetComponent<TMP_Dropdown>().ClearOptions();
        //     gangDropdown2.GetComponent<TMP_Dropdown>().ClearOptions();
        //     
        //     foreach (var gang in Gang.All) {
        //         gangDropdown1.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData(gang.Name));
        //         gangDropdown2.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData(gang.Name));
        //     }
        //     
        //     gangDropdown1.GetComponent<TMP_Dropdown>().value = 0;
        //     gangDropdown2.GetComponent<TMP_Dropdown>().value = 1;
        //     
        //     _selectedGang1 = Gang.All[0];
        //     _selectedGang2 = Gang.All[1];
        //     
        //     SetGangInfoPanel(Gang.All[0], gangInfoPanel1);
        //     SetGangInfoPanel(Gang.All[1], gangInfoPanel2);
        // }
        //
        // public void StartSkirmish() {
        //     BattleStartManager.Instance.StartBattle(new List<Gang> { _selectedGang1, _selectedGang2 });
        // }
        //
        // //public void SetPlayerControllerToggle1(bool toggle) => _selectedGang1.IsPlayerControlled = toggle;
        // //public void SetPlayerControllerToggle2(bool toggle) => _selectedGang2.IsPlayerControlled = toggle;
        //
        // public void SetGangDropdown(int dropdown) {
        //     var dropdownObject = dropdown == 1 ? gangDropdown1 : gangDropdown2;
        //     var selectedGang = dropdown == 1 ? _selectedGang1 : _selectedGang2;
        //     
        //     selectedGang = Gang.All[dropdownObject.GetComponent<TMP_Dropdown>().value];
        //     SetGangInfoPanel(selectedGang, dropdown == 1 ? gangInfoPanel1 : gangInfoPanel2);
        // }
        //
        // public void NextGangButton(int dropdown) {
        //     var dropdownObject = dropdown == 1 ? gangDropdown1 : gangDropdown2;
        //     var selectedGang = dropdown == 1 ? _selectedGang1 : _selectedGang2;
        //     
        //     var currentIndex = dropdownObject.GetComponent<TMP_Dropdown>().value;
        //     var newIndex = currentIndex + 1;
        //     if (newIndex >= Gang.All.Count) {
        //         newIndex = 0;
        //     }
        //     
        //     dropdownObject.GetComponent<TMP_Dropdown>().value = newIndex;
        //     selectedGang = Gang.All[newIndex];
        //     SetGangInfoPanel(selectedGang, dropdown == 1 ? gangInfoPanel1 : gangInfoPanel2);
        // }
        //
        // public void PreviousGangButton(int dropdown) {
        //     var dropdownObject = dropdown == 1 ? gangDropdown1 : gangDropdown2;
        //     var selectedGang = dropdown == 1 ? _selectedGang1 : _selectedGang2;
        //     
        //     var currentIndex = dropdownObject.GetComponent<TMP_Dropdown>().value;
        //     var newIndex = currentIndex - 1;
        //     if (newIndex < 0) {
        //         newIndex = Gang.All.Count - 1;
        //     }
        //     
        //     dropdownObject.GetComponent<TMP_Dropdown>().value = newIndex;
        //     selectedGang = Gang.All[newIndex];
        //     SetGangInfoPanel(selectedGang, dropdown == 1 ? gangInfoPanel1 : gangInfoPanel2);
        // }
        //
        // private void SetGangInfoPanel(Gang gang, GameObject gangInfoPanel) {
        //     gangInfoPanel.GetComponent<GangInfoPanel>().SetGang(gang);
        // }
    }
}