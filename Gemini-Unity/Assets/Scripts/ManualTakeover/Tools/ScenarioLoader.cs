using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenarioLoader : MonoBehaviour {
	public Dropdown dropdown;
	public int baseScene = 1;

	public void PlayScenario() {
		int scenarioScene = dropdown.value + baseScene + 1;

		SceneManager.sceneLoaded += (scene, _) => {
			if (scene.buildIndex == scenarioScene) {
				SceneManager.LoadScene(baseScene, LoadSceneMode.Additive);
			}
		};

		// Scenario scene must be loaded first, as the base scene depends on it
		// Scenario setup is initiated from GameController.Start (Present in the base scene)
		SceneManager.LoadScene(scenarioScene, LoadSceneMode.Single);
	}
}
