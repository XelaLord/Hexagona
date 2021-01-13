using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour {

	Shape shape;

	TextMeshProUGUI nameText;
	Image picture;

	Slider energySlider;
	TextMeshProUGUI creationCost;
	TextMeshProUGUI destructionCost;

	void Awake() {
		nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		picture = transform.Find("Picture").GetComponent<Image>();
		energySlider = GetComponentInChildren<Slider>();
		creationCost = energySlider.transform.Find("CreationCost").GetComponent<TextMeshProUGUI>();
		destructionCost = energySlider.transform.Find("DestructionCost").GetComponent<TextMeshProUGUI>();
	}
	
	public void SetShape(string shapeName) {
		shape = Shape.Load(shapeName);
		
		string path = Application.dataPath + "/Resources/Shapes/" + shapeName + ".png";
		Sprite image = LoadNewSprite(path);
		picture.sprite = image;

		nameText.text = shape.shapeName;
		energySlider.value = shape.creationEnergy;
		energySlider.maxValue = shape.creationEnergy + shape.destructionEnergy;
		UpdateSlider();
	}

	public void UpdateSlider() {
		creationCost.text = energySlider.value.ToString();
		destructionCost.text = (energySlider.maxValue - energySlider.value).ToString();
	}

	public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f) {
   
		// Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
		//Sprite NewSprite = new Sprite();
		Texture2D SpriteTexture = LoadTexture(FilePath);
		Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), PixelsPerUnit);

		return NewSprite;
   }
 
   public Texture2D LoadTexture(string FilePath) {
 
		// Load a PNG or JPG file from disk to a Texture2D
		// Returns null if load fails
		Texture2D Tex2D;
		byte[] FileData;

		if (File.Exists(FilePath)) {
			FileData = File.ReadAllBytes(FilePath);
			Tex2D = new Texture2D(2, 2);
			if (Tex2D.LoadImage(FileData)) {
				return Tex2D;
			}
		}
		return null;
	}
}