using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	public static bool isPaused = false;

	public void Toggle() {
		if (isPaused) {
			Resume();
		} else {
			Pause();
		}
	}

	public void Pause() {
		gameObject.SetActive(true);
		isPaused = true;
	}

	public void Resume() {
		gameObject.SetActive(false);
		isPaused = false;
	}

	public void Quit() {
		SceneLoader.Load(SceneLoader.Scene.StartMenu);
	}
}