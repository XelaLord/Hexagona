using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour {

	public GameUI UI;
	public PauseMenu pauseMenu;
	public HexGrid map;

	public int boardSize;
	public int turnEnergy;
	public int currentEnergy;
	int remainingPieceMoves;

	// State variables
	public enum Mode {
		Null,
		Selecting,
		Moving,
		Capturing
	}

	public Mode mode = Mode.Null;
	public string turnState = "white";
	bool needUpdate = false;

	void Awake() {
		map = GetComponentInChildren<HexGrid>();
	}

	void Start() {
		pauseMenu.Resume();
		UI.SetMaxEnergy(turnEnergy);
		turnState = "white";

		if (!LoadSave("Save")) {
			map.CreateBoard(boardSize);
		}

		InitialiseCameras();
		LoadFormations();
		UpdateBoard();
	}

	void Update() {
		// PAUSE MENU
		if (Input.GetKeyDown(KeyCode.Escape) && selectedPieces.Count == 0 && mode != Mode.Moving) {
			pauseMenu.Toggle();
		}

		if (!PauseMenu.isPaused) {

			// SELECTING
			if (Input.GetMouseButtonDown(0) && !UI.IsMouseOverUI()) {
				StartSelect();
			} else if (Input.GetMouseButtonUp(0)) {
				EndSelect();
			}

			// ESCAPE KEY
			if (Input.GetKeyDown(KeyCode.Escape) && mode == Mode.Null && selectedPieces.Count > 0) {
				ClearSelected();
			}

			// MOVING
			if (Input.GetKeyDown(KeyCode.M) && mode == Mode.Null && selectedPieces.Count > 0) {
				CreateGhosts();	// Start move
			}
			if (mode == Mode.Moving) {
				MoveGhosts();
				if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !UI.IsMouseOverUI()) {
					if (CanGhostsDrop()) {
						DropGhosts();
						if (Input.GetMouseButtonDown(1)) {
							CreateGhosts();
						}
					}
				}
				if (Input.GetKeyDown(KeyCode.Escape)) {
					CancelMove();
				}
			}

			// CAPTURING
			if (Input.GetKeyDown(KeyCode.C)) {
				if (mode == Mode.Null) {
					mode = Mode.Capturing;
					UpdateBoard();
				} else if (mode == Mode.Capturing) {
					mode = Mode.Null;
					UpdateBoard();
				}
			}

			// GROUPING
			if (Input.GetKeyDown(KeyCode.G) && mode == Mode.Null && selectedPieces.Count > 1) {
				bool isGroupSelected = false;
				foreach (var piece in selectedPieces) {
					isGroupSelected = piece.group || isGroupSelected;
				}

				if (isGroupSelected) {
					foreach (var piece in selectedPieces) {
						if (piece.group) {
							map.DeleteGroup(piece.group);
						}
					}
				} else {
					map.CreateGroup(selectedPieces.ToArray());
				}
			}

			// FORMATIONS
			if (Input.GetKeyDown(KeyCode.F) && mode == Mode.Null && selectedPieces.Count > 1) {

				bool didCreate = false;
				foreach (var potential in map.potentials) {
					if (Utils.CompareLists(potential.pieces, selectedPieces.ToArray())) {
						didCreate = AttemptEngage(potential);
						break;
					}
				}

				if (!didCreate) {
					foreach (var formation in map.formations) {
						if (Utils.CompareLists(formation.pieces, selectedPieces.ToArray())) {
							if (currentEnergy >= formation.shape.destructionEnergy) {
								map.DeleteFormation(formation);
								UpdateBoard();
								currentEnergy -= formation.shape.destructionEnergy;
							}
								break;
						}
					}
				}	
			}

			// ENTER KEY
			if (Input.GetKeyDown(KeyCode.Return) && mode == Mode.Null && selectedPieces.Count == 0) {
				EndTurn();
			}
		}

		// UPDATE BOARD
		if (needUpdate) {
			map.UpdateMap();
			ClearPotentials();
			if (mode == Mode.Null) {
				ScanForFormations(new List<Shape>(turnState == "white" ? whiteShapes : blackShapes).ToArray(), turnState);		
			}

			needUpdate = false;
		}
	}

	public void UpdateBoard() {
		needUpdate = true;
	}

	private void OnGUI() {
		Rect rect;
		if (Input.GetMouseButton(0) && mode == Mode.Selecting && (p1 - Input.mousePosition).magnitude > 40) {
			rect = Utils.GetScreenRect(p1, Input.mousePosition);
			Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.8f, 0.25f));
			Utils.DrawScreenRectBorder(rect, 1f, new Color(0.9f, 0.9f, 0.9f));
		}

		int energy = mode == Mode.Moving ? currentEnergy - MoveCost(displacement) : currentEnergy;
		UI.SetEnergy(energy);

		bool move = mode == Mode.Null && selectedPieces.Count > 0 || mode == Mode.Moving;
		bool capture = mode == Mode.Null || mode == Mode.Capturing;
		UI.modeSelector.SetEnabled(new bool[] {true, move, capture});

		switch (mode) {
			case Mode.Moving:
				UI.modeSelector.Select(1);
				break;
			case Mode.Capturing:
				UI.modeSelector.Select(2);
				break;
			default:
				UI.modeSelector.Select(0);
				break;
		}
	}

	public void SaveGame() {
		string path = Application.dataPath + "/Resources/Saves/Save.save";
		new FileInfo(path).Directory.Create();
		string json = JsonUtility.ToJson(new GameSave(this));		
		File.WriteAllText(path, json);
	}

	GameSave FindSave(string saveName) {
		string path = Application.dataPath + "/Resources/Saves/" + saveName + ".save";
		if (File.Exists(path)) {
			string json = File.ReadAllText(path);
			GameSave save = JsonUtility.FromJson<GameSave>(json);
			return save;
		} else {
			return null;
		}
	}

	public bool LoadSave(string saveName) {
		GameSave save = FindSave(saveName);
		if (save == null) {
			return false;
		}
		mode = Mode.Null;
		turnState = save.turnState ? "white" : "black";
		turnEnergy = save.turnEnergy;

		whiteCamera.SetActive(turnState == "white");
		blackCamera.SetActive(turnState == "black");

		ClearSelected();
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("WhitePieces"), turnState != "white");
		Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("BlackPieces"), turnState != "black");

		map.DeleteBoard();

		// TILES, PIECES, and TOWERS
		for (int i = 0; i < save.terrainKeys.Length; i++) {
			map.CreateTile(save.terrainKeys[i], save.terrainValues[i]);
		}
		for (int i = 0; i < save.pieceKeys.Length; i++) {
			map.CreatePiece(save.pieceKeys[i], save.pieceTeams[i] ? "white" : "black");
		}
		for (int i = 0; i < save.towerKeys.Length; i++) {
			map.CreateTower(save.towerKeys[i]);
		}

		// SHAPES and FORMATIONS
		this.whiteShapes = save.whiteShapes.Select(ShapeData => new Shape(ShapeData)).ToArray();
		this.blackShapes = save.blackShapes.Select(ShapeData => new Shape(ShapeData)).ToArray();
		Dictionary<string, Shape> shapeNames = new Dictionary<string, Shape>();
		System.Array.ForEach(whiteShapes, x => shapeNames[x.shapeName] = x);
		System.Array.ForEach(blackShapes, x => shapeNames[x.shapeName] = x);
		foreach (var data in save.formations) {
			GamePiece[] pieces = data.pieces.Select(i => map.pieces[i]).ToArray();
			Debug.Log(shapeNames[data.shapeName]);
			map.CreateFormation(pieces, shapeNames[data.shapeName]);
		}

		// GROUPS
		foreach (var data in save.groups) {
			GamePiece[] pieces = data.pieces.Select(i => map.pieces[i]).ToArray();
			map.CreateGroup(pieces);
		}


		map.UpdateOffenseMap();
		this.currentEnergy = save.currentEnergy;
		UpdateBoard();
		return true;
	}
}