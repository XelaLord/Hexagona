using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour {

	public GameManager GameManager;
	public Slider energyBar;
	public Selector modeSelector;

	void Awake() {
		modeSelector = GetComponentInChildren<Selector>();
		modeSelector.Select(0);
	}

	public void SetMaxEnergy(int max) {
		energyBar.maxValue = max;
	}

	public void SetEnergy(int energy) {
		if (energy > 0) {
			energyBar.value = energy;	
		} else {
			energyBar.value = 0;		
		}		
	}

	public bool IsMouseOverUI() {
		return EventSystem.current.IsPointerOverGameObject();
	}
}
