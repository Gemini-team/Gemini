using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public const int BOOT_SCENE = 0, MENU_SCENE = 1, BASE_SCENE = 2;

	private void Start() {
		FindObjectOfType<Scenario>().SetupScenario();
	}

	public void RestartGame() => SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Single);
}
