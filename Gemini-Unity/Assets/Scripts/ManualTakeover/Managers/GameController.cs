using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { BOOT_SCENE = 0, MENU_SCENE = 1, SHARED_SCENE = 2, RECORDING_SCENE = 3, BASE_SCENE = 4 }

public class GameController : MonoBehaviour {

	private void Start() {
		FindObjectOfType<Scenario>().SetupScenario();
	}

	public void ExitGame() => SceneManager.LoadScene((int)Scenes.MENU_SCENE, LoadSceneMode.Single);
}
