using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenarioLoader : MonoBehaviour {
	public Dropdown dropdown;

	public void PlayScenario() {
		int scenarioScene = dropdown.value + GameController.BASE_SCENE + 1;
		PlayerPrefs.SetInt("ScenarioScene", scenarioScene);

		// Scenario scene must be loaded first, as the base scene depends on it
		// Scenario setup is initiated from GameController.Start (Present in the base scene)
		SceneManager.LoadScene(scenarioScene, LoadSceneMode.Single);
	}
}
