using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour {

	public Text textBox;
	int timeCount = 0;
	bool isFirstUpdate = true;

	void Start() {
		InvokeRepeating("UpdateText", 0, 0.25f);
	}

	void Update() {
		if (isFirstUpdate) {
			isFirstUpdate = false;
		} else {
			SceneLoader.LoadCallback();
		}
	}

	void UpdateText() {
		if (timeCount == 18) {
			timeCount = 0;
		}

		string check = "LOADING" + (timeCount <= 12 ? "..." : "..");
		if (textBox.text == check) {
			textBox.text = "LOADING";
		} else {
			textBox.text += ".";
		}

		timeCount++;
	}
}
