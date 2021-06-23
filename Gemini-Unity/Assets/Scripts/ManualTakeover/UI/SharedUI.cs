using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedUI : MonoBehaviour {
    public ToggleableLayoutGroup menuGroup;

    public void ExitGame() => SceneManager.LoadScene(GameController.MENU_SCENE, LoadSceneMode.Single);

    private void Start() {
        menuGroup.Setup();

        if (PlayerPrefs.GetInt("GamePlayedOnce") == 0) {
            PlayerPrefs.SetInt("GamePlayedOnce", 1);
            menuGroup.Show("ControlsMenu");
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Pause")) {
            if (menuGroup.AnyVisible) menuGroup.HideAll();
            else menuGroup.Show("GameMenu");
        }
    }
}
