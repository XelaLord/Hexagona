using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : Collection {

	public Shape shape;
	public bool isPotential;

	public void Initialise(GamePiece[] pieces, Shape shape, bool isPotential=false) {
		this.pieces = pieces;
		this.shape = shape;
		this.isPotential = isPotential;
		color = isPotential ? Color.grey : Color.yellow;

		UpdateCollection();
	}

	public void DrawFlags() {
		foreach (var piece in pieces) {
			piece.AddFlag(shape.flagColor);
		}
	}

	public Vector2 Anchor() {
		return pieces[0].coords;
	}

	void OnDestroy() {
		foreach (var piece in pieces) {
			if (isPotential) {
				piece.potentials.Remove(this);
				map.potentials.Remove(this);
			} else {
				piece.formations.Remove(this);
				map.formations.Remove(this);
			}
		}
		Destroy(gameObject);
	}
}

[System.Serializable]
public class FormationData {

	public Vector2[] pieces;
	public string shapeName;

	public FormationData(Formation formation) {
		pieces = formation.pieces.Select(piece => piece.coords).ToArray();
		shapeName = formation.shape.shapeName;
	}
}