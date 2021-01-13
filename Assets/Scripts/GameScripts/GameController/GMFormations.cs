using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class GameManager {

	public Shape[] whiteShapes;
	public Shape[] blackShapes;
	
	void LoadFormations() {
		whiteShapes = Game.GetWhiteShapes().Select(x => Shape.Load(x)).ToArray();
		blackShapes = Game.GetBlackShapes().Select(x => Shape.Load(x)).ToArray();
	}

	void ClearPotentials() {
		foreach (var shape in map.potentials) {
			Destroy(shape.gameObject);
		}
		map.potentials.Clear();
	}
	
	void ScanForFormations(Shape[] shapes, string turn) {
		foreach (var piece in map.pieceList) {
			if (piece.team == turnState) {
				int sign = piece.team == "white" ? 1 : -1;
				foreach (var shape in shapes) {

					bool success = true;
					foreach (var relative in shape.positive) {
						Vector2 coords = piece.coords + sign*relative;
						success = (map.pieces.ContainsKey(coords) && map.pieces[coords].team == piece.team) && success;

						if (!success) {break;}
					}
					foreach (var relative in shape.negative) {
						Vector2 coords = piece.coords + sign*relative;
						success = !(map.pieces.ContainsKey(coords) && map.pieces[coords].team == piece.team) && success;

						if (!success) {break;}
					}
					foreach (var formation in map.formations) {
						success = !(shape.shapeName == formation.shape.shapeName && piece == formation.pieces[0]) && success; 

						if (!success) {break;}
					}

					if (success) {
						// Passed all tests, create potential
						List<GamePiece> pieces = new List<GamePiece>();
						foreach (var relative in shape.positive) {
							Vector2 coords = piece.coords + sign*relative;
							pieces.Add(map.pieces[coords]);
						}
						map.CreatePotential(pieces.ToArray(), shape);
					}
				}
			}
		}
	}

	bool AttemptEngage(Formation potential) {
		if (currentEnergy >= potential.shape.creationEnergy) {
			map.CreateFormation(potential.pieces, potential.shape);
			map.potentials.Remove(potential);
			Destroy(potential.gameObject);

			currentEnergy -= potential.shape.creationEnergy;
		}

		UpdateBoard();
		return currentEnergy >= potential.shape.creationEnergy;
	}
}