using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
	public Dropdown scenarioDropdown;
	public Dropdown recordingDropdown;

	private bool recordingsFound = false;

    private void Start() {
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		foreach (string path in Directory.GetDirectories(DataLogger.ROOT_DIRECTORY)) {
			options.Add(new Dropdown.OptionData(Path.GetFileName(path)));
        }

		if (options.Count == 0) options.Add(new Dropdown.OptionData("No recordings found"));
		else recordingsFound = true;
		recordingDropdown.AddOptions(options);
    }

	private void LoadSharedScene() => SceneManager.LoadScene(GameController.SHARED_SCENE, LoadSceneMode.Additive);

	public void PlayRecording() {
		if (!recordingsFound) return;

		PlayerPrefs.SetString("RecordingPath", Path.Combine(DataLogger.ROOT_DIRECTORY, recordingDropdown.options[recordingDropdown.value].text, "recording.csv"));
		SceneManager.LoadScene(GameController.RECORDING_SCENE, LoadSceneMode.Single);

		LoadSharedScene();
	}

	public void PlayScenario() {
		int scenarioScene = scenarioDropdown.value + GameController.BASE_SCENE + 1;
		PlayerPrefs.SetInt("ScenarioScene", scenarioScene);

		// Scenario scene must be loaded first, as the base scene depends on it
		// Scenario setup is initiated from GameController.Start (Present in the base scene)
		SceneManager.LoadScene(scenarioScene, LoadSceneMode.Single);

		LoadSharedScene();
	}
}
