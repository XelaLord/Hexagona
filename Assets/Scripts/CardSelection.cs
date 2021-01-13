using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelection : MonoBehaviour {

	public GameObject cardPrefab;
	public PauseMenu pauseMenu;

	void Start() {
		pauseMenu.Resume();
		CreateCard("Arrow");
		CreateCard("Builder");
		CreateCard("Digger");
		CreateCard("Point");
		CreateCard("Left Flank");
		CreateCard("Right Flank");
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenu.Toggle();
		}
	}

	void CreateCard(string shapeName) {
		GameObject newCard = Instantiate(cardPrefab, transform.Find("CardSelection"));
		Card card = newCard.GetComponent<Card>();
		card.SetShape(shapeName);
	}
}