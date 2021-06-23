using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public const int BOOT_SCENE = 0, MENU_SCENE = 1, RECORDING_SCENE = 2, BASE_SCENE = 3;

	private void Start() {
		FindObjectOfType<Scenario>().SetupScenario();
	}

	public void RestartGame() => SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Single);
}
