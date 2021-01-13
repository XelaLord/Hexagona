using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {

	public List<GamePiece> selectedPieces = new List<GamePiece>();
	RaycastHit hit;
	Vector3 p1;
	Vector3 p2;
	Collection selectedCollection;

	void StartSelect() {
		if (mode == Mode.Null && !UI.IsMouseOverUI()) {
			mode = Mode.Selecting;
			p1 = Input.mousePosition;
		}
	}

	void EndSelect() {
		if (mode == Mode.Selecting) {
			p2 = Input.mousePosition;
			mode = Mode.Null;						

			if ((p1 - p2).magnitude < 40) {
				PointSelect();
			} else {
				DragSelect();
			}
			p1 = Vector2.zero;
			p2 = Vector2.zero;

		} else if (mode == Mode.Capturing) {
			GameObject target = Raycast();
			if (target) {
				GamePiece piece = target.GetComponent<GamePiece>();
				AttemptCapture(piece);
			}
		}
	}

	void PointSelect() {
		GameObject target = Raycast();
		if (target) {
			GamePiece piece = target.GetComponent<GamePiece>();
			if (piece.team == turnState) {
				if (selectedPieces.Contains(piece) && !Input.GetKey(KeyCode.LeftControl)) {
					CycleCollection(piece);
				} else {
					if (!Input.GetKey(KeyCode.LeftControl))	{
						ClearSelected();
						selectedCollection = null;
					}
					piece.ToggleSelect();
				}
			}
		} else {
			ClearSelected();
			selectedCollection = null;
		}
	}

	void DragSelect() {
		if (!Input.GetKey(KeyCode.LeftControl))	{
			ClearSelected();
		}

		// GET INTERSECTIONS WITH GROUND PLANE
		Plane groundPlane = new Plane(Vector3.up, Vector3.up * -3);
		Vector2[] corners = GetBoundingBox(p1, p2);
		Vector3[] vertices = new Vector3[8];
		int[] triangles = {0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7};	
		
		for(int i = 0; i < 4; i++) {
			Ray ray = Camera.main.ScreenPointToRay(corners[i]);
			float dist = 0.0f;
			groundPlane.Raycast(ray, out dist);
			vertices[i] = ray.GetPoint(dist);
			vertices[i + 4] = ray.GetPoint(1);
			Debug.DrawLine(Camera.main.ScreenToWorldPoint(corners[i]), vertices[i], Color.red, 1.0f);
		}

		//Mesh selectionMesh = GenerateSelectionMesh(vertices);
		// GENERATE MESH
		Mesh selectionMesh = new Mesh();
		selectionMesh.vertices = vertices;
		selectionMesh.triangles = triangles;

		// CREATE COLLIDER
		MeshCollider selectionBox = gameObject.AddComponent<MeshCollider>();
		selectionBox.convex = true;
		selectionBox.isTrigger = true;
		selectionBox.sharedMesh = selectionMesh;

		Destroy(selectionBox, 0.2f);
	}

	void ClearSelected() {
		foreach(var piece in new List<GamePiece>(selectedPieces)) {
			piece.Deselect();
		}
	}

	void SelectAllEncumbered(GamePiece piece) {
		if (piece.group) {
			foreach (var p in piece.group.pieces) {
				if (!(selectedPieces.Contains(p))) {
					p.Select();
					SelectAllEncumbered(p);
				}
			}
		}
		foreach (var formation in piece.formations) {
			foreach (var p in formation.pieces) {
				if (!(selectedPieces.Contains(p))) {
					p.Select();
					SelectAllEncumbered(p);
				}
			}
		}
	}

	void CycleCollection(GamePiece piece) {
		ClearSelected();
		List<Collection> collections = new List<Collection>();
		if (piece.group != null) {
			collections.Add(piece.group);
		}
		foreach (var formation in piece.formations) {
			collections.Add(formation);
		}
		foreach (var potential in piece.potentials) {
			collections.Add(potential);
		}

		if (collections.Count > 0) {
			if (selectedCollection == null) {
				SelectCollection(collections[0]);
			} else {
				int i = collections.IndexOf(selectedCollection);
				i = (i + 1) % collections.Count;
				SelectCollection(collections[i]);
			}
		}
	}

	void SelectCollection(Collection collection) {
		foreach (var piece in collection.pieces) {
			piece.Select();
		}
		selectedCollection = collection;
	}

	GameObject Raycast() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask mask = LayerMask.GetMask("WhitePieces", "BlackPieces");

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
			GameObject target = hit.transform.gameObject;
			return target;
		}
		return null;
	}

	Vector2[] GetBoundingBox(Vector2 p1,Vector2 p2) {
		Vector2 newP1;
		Vector2 newP2;
		Vector2 newP3;
		Vector2 newP4;

		newP1 = new Vector2(Mathf.Min(p1.x, p2.x), Mathf.Max(p1.y, p2.y));	// Top left
		newP2 = new Vector2(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));	// Top right
		newP3 = new Vector2(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));	// Bottom left
		newP4 = new Vector2(Mathf.Max(p1.x, p2.x), Mathf.Min(p1.y, p2.y));	// Bottom right

		Vector2[] corners = {newP1, newP2, newP3, newP4};
		return corners;
	}

	Mesh GenerateSelectionMesh(Vector3[] corners) {
		Vector3[] vertices = new Vector3[8];
		int[] triangles = {0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7}; //HexGrid the tris of our cube

		for(int i = 0; i < 4; i++) {
			vertices[i] = corners[i] - 5*Vector3.up;
			vertices[i + 4] = corners[i] + 5*Vector3.up;
		}

		Mesh selectionMesh = new Mesh();
		selectionMesh.vertices = vertices;
		selectionMesh.triangles = triangles;

		return selectionMesh;
	}

	private void OnTriggerEnter(Collider other) {
		if (mode == Mode.Null) {
			GamePiece piece = other.gameObject.GetComponent<GamePiece>();
			piece.Select();
		}
	}
}
