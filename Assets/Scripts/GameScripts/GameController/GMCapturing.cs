using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {

	public void OnClickSelect() {
		if (mode == Mode.Moving) {
			CancelMove();
		} else if (mode == Mode.Capturing) {
			mode = Mode.Null;
			UpdateBoard();
		}
	}

	public void OnClickMove() {
		if (mode == Mode.Null && selectedPieces.Count > 0 ) {
			mode = Mode.Moving;
			CreateGhosts();
		}
	}

	public void OnClickCapture() {
		if (mode == Mode.Null) {
			mode = Mode.Capturing;
			UpdateBoard();
		}		
	}	

	bool AttemptCapture(GamePiece piece) {
		if (piece.IsVulnerable()) {
			int energyRequired = (turnState=="white") ? map.whiteOffense[piece.coords] : map.blackOffense[piece.coords];
			energyRequired = 5 * (3 - energyRequired);
			foreach (var formation in piece.formations) {
				energyRequired += formation.shape.destructionEnergy;
			}
			energyRequired = Mathf.Max(0, energyRequired);

			if (currentEnergy >= energyRequired) {
				for (int i = 0; i < piece.formations.Count; i++) {
					map.DeleteFormation(piece.formations[i]);
					UpdateBoard();
				}

				currentEnergy -= energyRequired;
				Destroy(piece);
				UpdateBoard();

				return true;
			}
		}
		return false;
	}
}