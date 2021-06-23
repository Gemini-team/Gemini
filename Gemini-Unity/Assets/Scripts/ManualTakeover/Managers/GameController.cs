using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public const int BOOT_SCENE = 0, MENU_SCENE = 1, SHARED_SCENE = 2, RECORDING_SCENE = 3, BASE_SCENE = 4;

	private void Start() {
		FindObjectOfType<Scenario>().SetupScenario();
	}

	public void ExitGame() => SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Single);
}
