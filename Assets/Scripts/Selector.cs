using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour {

	Button[] buttons;
	public GameObject selector;
	int selected;

	void Awake() {
		buttons = GetComponentsInChildren<Button>();

		if (selector == null) {
			selector = transform.Find("Selector").gameObject;
		}
	}

	public void Select(int item) {
		selector.transform.position = buttons[item].transform.position;
		selected = item;
	}

	public void SetEnabled(bool[] bools = null) {
		if (bools == null) {
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].interactable = (i == selected);
			}
		} else if (bools.Length == buttons.Length) {
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].interactable = bools[i];
			}
		}
	}
}