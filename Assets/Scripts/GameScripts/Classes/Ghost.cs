using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePiece))]
public class Ghost : MonoBehaviour {

	HexGrid map;
	public GamePiece piece;
	public Vector2 homeCoords;
	public Vector2 currentCoords;

	void Awake() {
		piece = gameObject.GetComponent<GamePiece>();
		map = GetComponentInParent<HexGrid>();
		homeCoords = piece.coords;
		currentCoords = homeCoords;
		map.pieces.Remove(homeCoords);
	}

	public void UpdatePos(Vector2 displacement) {
		currentCoords = homeCoords + displacement;
		if (map.tiles.ContainsKey(currentCoords)) {
			gameObject.GetComponent<MeshRenderer>().enabled = true;
			piece.coords = currentCoords;
			transform.position = map.tiles[currentCoords].GetTop() + Vector3.up;
		} else {
			gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public bool CanDrop() {
		Vector2 coords = currentCoords;
		if (map.tiles.ContainsKey(coords)) {
			if (map.pieces.ContainsKey(coords) || map.towers.ContainsKey(coords)) {
				return false; 
			} else {
				return true;
			}
		}
		return false;
	}

	public void Drop() {
		piece.MoveTo(currentCoords);
		piece.Select();
		Destroy(this);
	}

	public void Cancel() {
		piece.MoveTo(homeCoords);
		piece.Select();
		piece.gameObject.SetActive(true);
		Destroy(this);
	}
}
