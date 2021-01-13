using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {

	public enum Scene {
		StartMenu,
		SettingsMenu,
		GameScene,
		FormationEditor,
		CardSelection,
		Loading
	}

	private static System.Action onLoadCallback;

	public static void Load(Scene scene) {
		onLoadCallback = () => {
			SceneManager.LoadScene(scene.ToString());
		};
		SceneManager.LoadScene(Scene.Loading.ToString());
	}

	public static void LoadCallback() {
		if (onLoadCallback != null) {
			onLoadCallback();
			onLoadCallback = null;
		}
	}
}
