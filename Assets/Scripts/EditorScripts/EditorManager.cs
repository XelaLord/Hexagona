using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorManager : MonoBehaviour {

	public HexGrid map;
	public PauseMenu pauseMenu;
	public EditorUI UI;
	public CameraController cam;
	public EditorCamera pictureCamera;
	public static Shape shape;

	public enum Mode {
		Pieces,
		Offense,
		Defense,
		Elevation
	}
	public Mode editMode = Mode.Pieces;

	void Start() {
		map = GetComponentInChildren<HexGrid>();
		shape = new Shape();
		shape.positive = new Vector2[1] {Vector2.zero};
		LoadShape(shape);
	}

	void Update() {

		cam.inputActive = !UI.NameFocused();

		// EDITING
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !EventSystem.current.IsPointerOverGameObject()) {

			GameObject target = Raycast();
			if (target != null) {
				Hex tile = target.GetComponent<Hex>();
				int sign = Input.GetMouseButtonDown(0) ? 1 : -1;

				// PIECES
				if (editMode == Mode.Pieces) {
					if (tile.coords != Vector2.zero) {
						if (map.pieces.ContainsKey(tile.coords)) {
							GamePiece piece = map.pieces[tile.coords];
							int team = piece.team == "white" ? 1 : -1;

							if (sign != team) {
								Destroy(piece);
							}
						} else {
							string team = sign == 1 ? "white" : "black";
							map.CreatePiece(tile.coords, team);
							map.pieces[tile.coords].SetColor(team == "white" ? Color.white : Color.red);
						}
					}
				}

				// OFFENSE
				if (editMode == Mode.Offense) {
					if (shape.offense.ContainsKey(tile.coords)) {
						shape.offense[tile.coords] += sign;
					} else {
						shape.offense.Add(tile.coords, sign);
					}

					shape.offense[tile.coords] = Mathf.Max(0, shape.offense[tile.coords]);
					if (shape.offense[tile.coords] == 0) {
						shape.offense.Remove(tile.coords);
					}
				}

				// DEFENSE
				if (editMode == Mode.Defense) {
					if (shape.defense.ContainsKey(tile.coords)) {
						shape.defense[tile.coords] += sign;
					} else {
						shape.defense.Add(tile.coords, sign);
					}

					shape.defense[tile.coords] = Mathf.Max(0, shape.defense[tile.coords]);
					if (shape.defense[tile.coords] == 0) {
						shape.defense.Remove(tile.coords);
					}
				}

				// TERRAIN
				if (editMode == Mode.Elevation) {
					if (shape.terrain.ContainsKey(tile.coords)) {
						shape.terrain[tile.coords] += sign;
					} else {
						shape.terrain.Add(tile.coords, sign);
					}

					shape.terrain[tile.coords] = Mathf.Clamp(shape.terrain[tile.coords], -5, 5);
					tile.SetElevation(shape.terrain[tile.coords]);
					if (shape.terrain[tile.coords] == 0) {
						shape.terrain.Remove(tile.coords);
					}

					foreach (var piece in map.pieceList) {
						piece.UpdatePosition();
					}
				}
			}
		}

		// OTHER INPUTS
		if (Input.GetKeyDown(KeyCode.Space)) {
			AdvanceEditMode();
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenu.Toggle();
		}
		

		// UPDATING ENERGY FROM TERRAIN
		int totalElevation = 0;
		foreach (var h in shape.terrain.Values) {
			totalElevation += Mathf.Abs(h);
		}
		UI.SetSliderValues(shape.creationEnergy, totalElevation * 4);

		UpdateRings();
	}

	void LoadShape(Shape loadShape) {
		map.DeleteBoard();

		for (int x = -10; x < 10; x++) {
			for (int y = -10; y < 10; y++) {
				float yOff = x % 2 == 0 ? 0f : 0.5f;
				Vector2 coords = new Vector2(x, y + yOff);
				map.CreateTile(coords, 0);
			}
		}

		foreach (var coords in loadShape.positive) {
			map.CreatePiece(coords, "white");
		}
		foreach (var coords in loadShape.negative) {
			map.CreatePiece(coords, "black");
			map.pieces[coords].SetColor(Color.red);
		}
		foreach (var tile in loadShape.terrain) {
			map.tiles[tile.Key].SetElevation(tile.Value);
		}

		UI.SetName(loadShape.shapeName);
		UI.UpdateDropdown();
		shape = loadShape;
	}

	void UpdateShape() {
		List<Vector2> positive = new List<Vector2>();
		List<Vector2> negative = new List<Vector2>();
		foreach (var piece in map.pieceList) {
			if (piece.team == "white") {
				positive.Add(piece.coords);
			} else {
				negative.Add(piece.coords);
			}
		}

		if (positive.Count > 0) {
			if (positive[0] != Vector2.zero || true) {
				int index = positive.IndexOf(Vector2.zero);

				positive[index] = positive[0];
				positive[0] = Vector2.zero;
			}
		}

		shape.positive = positive.ToArray();
		shape.negative = negative.ToArray();
	}

	public void SaveShape() {
		if (shape.shapeName != null && shape.shapeName != "") {
			UpdateShape();
			Shape.Save(shape);
			UI.UpdateDropdown();
		}
	}

	void UpdateRings() {
		foreach (var tile in map.tiles.Values) {
			int offense = shape.offense.ContainsKey(tile.coords) ? shape.offense[tile.coords] : 0;
			int defense = shape.defense.ContainsKey(tile.coords) ? shape.defense[tile.coords] : 0;
			DrawRings(tile, offense, defense);
		}
	}

	void DrawRings(Hex tile, int offense, int defense) {
		for (int i = 0; i < tile.transform.childCount; i++) {
			Destroy(tile.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < offense + defense; i++) {
			GameObject ring = new GameObject("Ring", typeof(LineRenderer), typeof(Ring));
			ring.transform.parent = tile.transform;
			ring.transform.position = tile.GetTop();
			ring.transform.Translate((i + 1) * new Vector3(0, 0.2f, 0));

			Color color = i < defense ? Color.blue : Color.red;
			ring.GetComponent<Ring>().DrawRing(0.7f, 0.05f, color);	
		}
	}

	public void OnLoadButtonPressed() {
		name = UI.GetSelectedFilename();
		if (name != "") {
			Shape shape = Shape.Load(name);
			LoadShape(shape);
			//PrintBoardState();
		} else {
			shape = new Shape();
			shape.positive = new Vector2[1] {Vector2.zero};
			LoadShape(shape);
		}		
	}

	void PrintBoardState() {
		Debug.Log("Printing Board State");
		Debug.Log(map.pieces.Count + " : " + map.pieceList.Count);
		foreach (var piece in map.pieces) {
			Debug.Log(piece.Key.ToString() + ": " + piece.Value);
		}
	}

	public void OnPicture() {
		UpdateShape();
		pictureCamera.TakePicture(shape);
	}

	void AdvanceEditMode() {
		editMode = (Mode)(((int)editMode + 1) % 4);
		UI.SetMode(editMode);
	}

	GameObject Raycast() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask mask = LayerMask.GetMask("Tiles");

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask)) {
			GameObject target = hit.transform.gameObject;
			return target;
		}
		return null;
	}
}