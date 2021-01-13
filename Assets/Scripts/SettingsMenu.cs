using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	public void SetQuality(int qualityIndex) {
		QualitySettings.SetQualityLevel(qualityIndex);
	}

	public void SetFullscreen(bool isFullscreen) {
		Screen.fullScreen = isFullscreen;
	}

	public void OnBack() {
		SceneLoader.Load(SceneLoader.Scene.StartMenu);
	}
}