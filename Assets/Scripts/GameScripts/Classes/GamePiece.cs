using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

	GameManager game;
	HexGrid map;

	public string team;
	public Vector2 coords;
	public Group group;
	public List<Formation> formations = new List<Formation>();
	public List<Formation> potentials = new List<Formation>();
	public List<Color> flagColors = new List<Color>();

	public Dictionary<Vector2, int> offense = new Dictionary<Vector2, int>(){
		[new Vector2(-1,0.5f)] = 1,
		[new Vector2(0,1f)] = 1,
		[new Vector2(1,0.5f)] = 1,
	};
	
	void Awake() {
		game = GetComponentInParent<GameManager>();
		map = GetComponentInParent<HexGrid>();
		name = "GamePiece " + map.pieceList.Count;
	}

	public void Initialise(Vector2 coords, string team) {
		this.team = team;
		gameObject.layer = team=="white" ? LayerMask.NameToLayer("WhitePieces") : LayerMask.NameToLayer("BlackPieces");
		this.coords = coords;
		if (!map.pieces.ContainsKey(coords)) {
			map.pieces.Add(coords, this);
			map.pieceList.Add(this);
		}
		
		UpdatePosition();
		SetColor();
	}

	public bool IsVulnerable() {
		int offenseValue = team == "white" ? map.blackOffense[coords] : map.whiteOffense[coords];
		return offenseValue > 0;
	}

	public void Move(Vector2 displacement) {
		MoveTo(coords + displacement);
	}

	public void MoveTo(Vector2 coords) {
		if (!map.pieces.ContainsKey(coords)) {
			map.pieces.Add(coords, this);	
		} else {
			Debug.Log("Trying to overwrite a piece here");
		}

		this.coords = coords;		
		UpdatePosition();
	}

	public void UpdatePosition() {
		transform.position = map.tiles[coords].GetTop() + new Vector3(0, 0.15f, 0);
	}

	public void SetColor(Color? color = null) {
		if ((color ?? Color.clear) != Color.clear) {
			GetComponent<Renderer>().material.color = (Color)color;
		} else { 
			GetComponent<Renderer>().material.color = team == "white" ? Color.white : Color.black;
		}
	}

	public void AddFlag(Color color) {
		/*GameObject flagObject = Instantiate(map.flagPrefab, transform);
		flagObject.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
		flagObject.transform.rotation = Quaternion.identity;
		flagObject.transform.Rotate(0, flagColors.Count * 60, 0);
		flagColors.Add(color);*/
	}

	public void DestroyFlags() {
		for (int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
		flagColors = new List<Color>();
	}

	public List<GamePiece> GetNeighbors() {
		List<GamePiece> neighbors = new List<GamePiece>();

		foreach (var relative in map.relatives) {
			if (map.pieces.ContainsKey(coords + relative)) {
				neighbors.Add(map.pieces[coords + relative]);
			}
		}
		return neighbors;
	}

	public void ToggleSelect() {
		if (game.selectedPieces.Contains(this)) {
			Deselect();
		} else {
			Select();
		}
	}

	public void Select() {
		if (!game.selectedPieces.Contains(this)) {
			game.selectedPieces.Add(this);
			SetColor(Color.magenta);
		}
	}

	public void Deselect() {
		if (game.selectedPieces.Contains(this)) {
			game.selectedPieces.Remove(this);
			SetColor();
		}		
	}

	void OnDestroy() {
		map.pieces.Remove(coords);
		map.pieceList.Remove(this);
		Destroy(gameObject);
	}
}