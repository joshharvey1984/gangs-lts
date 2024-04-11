using Gangs.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gangs {
    public class MainMenu : MonoBehaviour {
        private void Awake() {
            DataManager.CreateData();
        }

        public void StartGame() {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
