using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour {

	public void OnStartGame() {
		SceneLoader.Load(SceneLoader.Scene.GameScene);
	}

	public void OnEditor() {
		SceneLoader.Load(SceneLoader.Scene.FormationEditor);
	}

	public void OnCardSelection() {
		SceneLoader.Load(SceneLoader.Scene.CardSelection);
	}

	public void OnSettings() {
		SceneLoader.Load(SceneLoader.Scene.SettingsMenu);	
	}

	public void OnQuit() {
		Application.Quit();
	}
}
