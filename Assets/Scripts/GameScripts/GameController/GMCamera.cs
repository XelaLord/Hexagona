using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {

	public CameraController whiteCamera;
	public CameraController blackCamera;	

	void InitialiseCameras() {
		currentEnergy = turnEnergy;
		whiteCamera.SetActive(turnState == "white");
		blackCamera.SetActive(turnState == "black");
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("WhitePieces"), turnState != "white");
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("BlackPieces"), turnState != "black");
	}

	void EndTurn() {
		mode = Mode.Null;
		currentEnergy = turnEnergy;
		if (turnState == "white") {
			turnState = "black";
		} else {
			turnState = "white";
		}
		whiteCamera.SetActive(turnState == "white");
		blackCamera.SetActive(turnState == "black");

		ClearSelected();
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("WhitePieces"), turnState != "white");
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("BlackPieces"), turnState != "black");

		map.CheckTowers(turnState);
		UpdateBoard();
		SaveGame();
	}
}

