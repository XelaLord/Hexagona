using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HexGrid {
	
	public Dictionary<Vector2, int> whiteOffense = new Dictionary<Vector2, int>();
	public Dictionary<Vector2, int> blackOffense = new Dictionary<Vector2, int>();

	void DrawRings() {
		foreach (var tile in tiles) {
			if (NotAllied(tile.Key, game.turnState) && game.mode != GameManager.Mode.Capturing) {
				int rings = game.turnState == "white" ? whiteOffense[tile.Key] : blackOffense[tile.Key];
				tile.Value.DrawRings(rings);
			} else {
				tile.Value.DrawRings(0);
			}
		}
	}

	void ColorPieces() {
		foreach (var piece in pieceList) {
			if (game.mode == GameManager.Mode.Moving && piece.GetComponent<Ghost>() && !piece.GetComponent<Ghost>().CanDrop()) {
				piece.SetColor(Color.red);
			} else if (game.mode == GameManager.Mode.Capturing && piece.team != game.turnState && piece.IsVulnerable()) {
				piece.SetColor(Color.yellow);
			} else if (game.selectedPieces.Contains(piece)) {
				piece.SetColor(Color.magenta);
			} else {
				piece.SetColor();
			}
		}
	}

	public void UpdateOffenseMap() {
		// Reset dictionarys
		whiteOffense.Clear();
		blackOffense.Clear();
		foreach (var coords in tiles.Keys) {
			whiteOffense.Add(coords, 0);
			blackOffense.Add(coords, 0);
		}

		// Pieces

		foreach (var piece in pieceList) {
			int sign = piece.team == "white" ? 1 : -1;
			foreach (var offense in piece.offense) {
				Vector2 coords = piece.coords + sign*offense.Key;
				if (tiles.ContainsKey(coords)) {
					int elevationAdvantage = tiles[piece.coords].elevation - tiles[coords].elevation;
					if (Mathf.Abs(elevationAdvantage) <= 2) {
						int offValue = offense.Value + elevationAdvantage;
						if (piece.team == "white") {
							whiteOffense[coords] += offValue;
						} else {
							blackOffense[coords] += offValue;
						}
					}
				}
			}
		}


		// Formations
		foreach (var formation in formations) {
			int sign = formation.pieces[0].team == "white" ? 1 : -1;
			// Offense
			foreach (var offense in formation.shape.offense) {
				Vector2 coords = formation.Anchor() + sign*offense.Key;
				if (tiles.ContainsKey(coords)) {
					int elevationAdvantage = tiles[formation.Anchor()].elevation - tiles[coords].elevation;
					if (Mathf.Abs(elevationAdvantage) <= 2) {
						if (sign == 1) {
							whiteOffense[coords] += offense.Value;
						} else {
							blackOffense[coords] += offense.Value;
						}
					}
				}
			}
			// Defense
			foreach (var defense in formation.shape.defense) {
				Vector2 coords = formation.Anchor() + sign*defense.Key;
				if (tiles.ContainsKey(coords)) {
					int elevationAdvantage = tiles[formation.Anchor()].elevation - tiles[coords].elevation;
					if (Mathf.Abs(elevationAdvantage) <= 2) {
						if (sign == 1) {
							blackOffense[coords] -= defense.Value;
						} else {
							whiteOffense[coords] -= defense.Value;
						}
					}
				}
			}
		}
	}
}