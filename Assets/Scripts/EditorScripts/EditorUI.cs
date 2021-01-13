using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour {

	public EditorManager manager;
	public PauseMenu pauseMenu;

	Selector modeSelector;

	Slider slider;
	Text creationCost;
	Text destructionCost;

	InputField nameInput;
	Text nameText;

	Dropdown fileDropdown;

	void Awake() {
		pauseMenu.Resume();

		modeSelector = GetComponentInChildren<Selector>();
		slider = transform.Find("Slider").GetComponent<Slider>();
		creationCost = slider.transform.Find("Creation Cost").GetComponent<Text>();
		destructionCost = slider.transform.Find("Destruction Cost").GetComponent<Text>();
		nameText = transform.Find("NameInput").transform.Find("Text").GetComponent<Text>();
		nameInput = GetComponentInChildren<InputField>();

		fileDropdown = transform.Find("FileDropdown").GetComponent<Dropdown>();
		UpdateDropdown();		
	}

	public void UpdateDropdown() {
		string path = Application.dataPath + "/Resources/Shapes/";
		new FileInfo(path).Directory.Create();

		string[] files = Directory.GetFiles(path, "*.shp");
		List<string> fileNames = new List<string>() {""};
		foreach (var file in files) {
			string filename = file.Substring(path.Length, file.Length - path.Length - 4);
			fileNames.Add(filename);
		}

		fileDropdown.ClearOptions();
		fileDropdown.AddOptions(fileNames);
	}
	
	public void SetMode(EditorManager.Mode mode) {
		modeSelector.Select((int)mode);
		manager.editMode = mode;
	}

	public void SetName(string name) {
		nameInput.text = name;
	}

	public void OnSliderChanged() {
		EditorManager.shape.creationEnergy = (int)slider.value;
		EditorManager.shape.destructionEnergy = (int)slider.maxValue - (int)slider.value;

		creationCost.text = slider.value.ToString();
		destructionCost.text = (slider.maxValue - slider.value).ToString();
	}

	public void SetSliderValues(int value, int max) {
		slider.maxValue = max;
		slider.value = value;

		OnSliderChanged();
	}

	public void OnNameChanged() {
		EditorManager.shape.shapeName = nameText.text;
	}

	public bool NameFocused() {
		return nameInput.isFocused;
	}

	public string GetSelectedFilename() {
		return fileDropdown.options[fileDropdown.value].text;
	}

	public void SelectPieces() {
		SetMode(EditorManager.Mode.Pieces);
	}

	public void SelectOffense() {
		SetMode(EditorManager.Mode.Offense);
	}

	public void SelectDefense() {
		SetMode(EditorManager.Mode.Defense);
	}

	public void SelectElevation() {
		SetMode(EditorManager.Mode.Elevation);
	}
}