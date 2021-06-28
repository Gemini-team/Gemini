using UnityEngine;
using UnityEngine.SceneManagement;

public class OnApplicationStart : MonoBehaviour {
    private void Start() {
        SceneManager.sceneLoaded += (scene, _) => {
            if (PlayerPrefs.HasKey("ScenarioScene") && scene.buildIndex == PlayerPrefs.GetInt("ScenarioScene")) {
                Debug.Log("Scenario loaded. Loading game...");
                SceneManager.LoadScene((int)Scenes.BASE_SCENE, LoadSceneMode.Additive);
            }
        };

        SceneManager.LoadScene((int)Scenes.MENU_SCENE, LoadSceneMode.Single);
    }
}
