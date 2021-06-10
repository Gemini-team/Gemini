using UnityEngine;
using UnityEngine.SceneManagement;

public class OnApplicationStart : MonoBehaviour {
    private void Start() {
        SceneManager.sceneLoaded += (scene, _) => {
            if (PlayerPrefs.HasKey("ScenarioScene") && scene.buildIndex == PlayerPrefs.GetInt("ScenarioScene")) {
                Debug.Log("Scenario loaded. Loading game...");
                SceneManager.LoadScene(GameController.BASE_SCENE, LoadSceneMode.Additive);
            }
        };

        SceneManager.LoadScene(GameController.MENU_SCENE, LoadSceneMode.Single);
    }
}
