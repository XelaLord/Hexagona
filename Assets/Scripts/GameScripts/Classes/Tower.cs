using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

	public Vector2 coords;
	
	HexGrid HexGrid;

	void Awake() {
		HexGrid = transform.parent.parent.GetComponent<HexGrid>();
		name = "Tower";
	}

	public void Initialise(Vector2 coords) {
		this.coords = coords;
		if (HexGrid.tiles.ContainsKey(coords)) {
			transform.position = HexGrid.tiles[coords].GetTop();
		}
	}

	public bool IsCaptured(string team) {
		int count = 0;
		foreach (var relative in HexGrid.relatives) {
			Vector2 tile = coords + relative;
			if (HexGrid.tiles.ContainsKey(tile) && HexGrid.pieces.ContainsKey(tile)) {
				if (HexGrid.pieces[tile].team == team) {
					count++;
				}
			}
		}

		return count >= 3;
	}

	public void AddPieces(string team) {
		foreach (var relative in HexGrid.relatives) {
			Vector2 tile = coords + relative;
			if (!HexGrid.pieces.ContainsKey(tile)) {
				HexGrid.CreatePiece(tile, team);
			}
		}
	}

	void OnDestroy() {
		Destroy(gameObject);
	}
}
