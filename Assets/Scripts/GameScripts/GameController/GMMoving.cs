using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {

	public List<Ghost> ghostPieces = new List<Ghost>();
	Hex anchorTile = null;
	Hex previousTile = null;
	Vector2 displacement;
	List<Vector2> path = new List<Vector2>();
	
	void CreateGhosts() {
		mode = Mode.Moving;
		foreach (var piece in new List<GamePiece>(selectedPieces)) {
			SelectAllEncumbered(piece);
		}

		foreach (var piece in selectedPieces) {
			Ghost ghost = piece.gameObject.AddComponent<Ghost>();
			ghostPieces.Add(ghost);

			if (anchorTile == null || piece.coords.y < anchorTile.coords.y) {
				anchorTile = map.tiles[piece.coords];
			}
		}
		previousTile = null;
		MoveGhosts();
	}

	void MoveGhosts() {
		Hex tile = HoveredOverTile();
		if (tile != null && tile != previousTile) {
			displacement = tile.coords - anchorTile.coords;
			previousTile = tile;

			foreach (var ghost in ghostPieces) {
				ghost.UpdatePos(displacement);
			}

			bool success = CanGhostsDrop();
			DrawRoute(success ? Color.white : Color.red);
			UpdateBoard();
		}
	}

	bool CanGhostsDrop() {
		bool success = true;
		foreach (var ghost in ghostPieces) {
			success = ghost.CanDrop() && success;
		}
		success = MoveCost(displacement)<=currentEnergy && success;
		success = CheckRoute(anchorTile.coords, displacement) && success;
		return success;
	}

	void DropGhosts() {
		foreach (var ghost in ghostPieces) {
			ghost.Drop();
		}
		currentEnergy += -MoveCost(displacement);
		DeleteGhosts();
	}

	void CancelMove() {
		foreach (var ghost in ghostPieces) {
			ghost.Cancel();
		}
		DeleteGhosts();
	}

	void DeleteGhosts() {
		mode = Mode.Null;
		ghostPieces.Clear();
		anchorTile = null;
		displacement = Vector2.zero;

		path = new List<Vector2>();
		DrawRoute(Color.white);
		UpdateBoard();
	}

	int MoveCost(Vector2 dis) {
		float xCost = Mathf.Abs(dis.x);
		float yCost = Mathf.Max(0, Mathf.Abs(dis.y) - Mathf.Abs(dis.x)*0.5f);

		return ElevationCost() + (int)(ghostPieces.Count * (xCost + yCost));
	}

	bool CheckRoute(Vector2 start, Vector2 dis) {
		Vector2 end = start + dis;

		// FINDING PATH
		path = new List<Vector2>();
		int directionX = start.x < end.x ? 1 : -1;
		int directionY = start.y < end.y ? 1 : -1;
		int verticalMoves = (int)(Mathf.Abs(dis.y) - Mathf.Abs(dis.x)*0.5f);
		int moves = Mathf.Max(0, verticalMoves) + (int)Mathf.Abs(dis.x);
		float x = start.x;
		float y = start.y;
		path.Add(new Vector2(x, y));

		for (int i = 0; i < moves; i++) {
			directionX = x < end.x ? 1 : -1;
			directionY = y < end.y ? 1 : -1;
			if (Mathf.Abs(y - end.y) > Mathf.Abs(x - end.x)) {
				// Move Vertical
				y += directionY;
				path.Add(new Vector2(x, y));
			} else {
				// Move Diagonal
				x += directionX;
				y += directionY * 0.5f;
				path.Add(new Vector2(x, y));
			}
		}

		// CHECKING PATH
		bool success = true;
		for (int i = 0; i < path.Count - 1; i++) {
			foreach (var ghost in ghostPieces) {
				Vector2 relative = ghost.homeCoords - start;
				if (map.tiles.ContainsKey(path[i] + relative) && map.tiles.ContainsKey(path[i + 1] + relative)) {
					int elevationDiff = Mathf.Abs(map.tiles[path[i] + relative].elevation -  map.tiles[path[i + 1] + relative].elevation);	
					success = success && elevationDiff <= 2;
				} else {
					success = false;
				}

				if (!success) {
					break;
				}
			}
		}

		return success;
	}

	void DrawRoute(Color color) {
		foreach (var line in GetComponentsInChildren<LineRenderer>()) {
			Destroy(line.gameObject);
		}

		for (int i = 0; i < path.Count - 1; i++) {
			DrawLink(map.tiles[path[i]], map.tiles[path[i + 1]], color);
		}
	}

	void DrawLink(Hex start, Hex end, Color color) {
		GameObject lineObject = new GameObject("Link", typeof(LineRenderer));
		lineObject.transform.parent = this.transform;
		LineRenderer line = lineObject.GetComponent<LineRenderer>();
		Vector3[] positions = {start.GetTop() + Vector3.up, end.GetTop() + Vector3.up};
		line.SetPositions(positions);
		line.useWorldSpace = false;
		line.startWidth = 0.1f;
		line.endWidth = 0.1f;
		line.material = new Material(Shader.Find("Standard"));
		line.GetComponent<Renderer>().material.color = color;
		line.generateLightingData = true;
	}

	int ElevationCost() {
		int cost = 0;
		foreach (var ghost in ghostPieces) {
			if (map.tiles.ContainsKey(ghost.currentCoords) && map.tiles.ContainsKey(ghost.homeCoords)) {
				cost += map.tiles[ghost.currentCoords].elevation - map.tiles[ghost.homeCoords].elevation;
			}
		}

		return cost;
	}

	Hex HoveredOverTile() {
		LayerMask mask = LayerMask.GetMask("Tiles");
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Hex tile = null;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
			tile = hit.transform.gameObject.GetComponent<Hex>();
		}

		return tile;
	}

	float[] Range(float start, float end) {
		int length = (int)Mathf.Abs(start-end) + 1;
		float[] range = new float[length];
		int direction = start < end ? 1 : -1;

		for (int i = 0; i < length; i++) {
			range[i] = start + i*direction;
		}

		return range;
	}
}
