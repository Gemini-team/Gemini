using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public void RestartGame() => SceneManager.LoadScene(0);
	public void QuitGame() => Application.Quit();
}
