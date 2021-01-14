using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HexGrid : MonoBehaviour {
	
	GameManager game;
	public GameObject hexPrefab;
	public GameObject piecePrefab;
	public GameObject flagPrefab;
	public GameObject towerPrefab;

	public Dictionary<Vector2, Hex> tiles = new Dictionary<Vector2, Hex>();
	public Dictionary<Vector2, GamePiece> pieces = new Dictionary<Vector2, GamePiece>();
	public List<GamePiece> pieceList = new List<GamePiece>();
	public Dictionary<Vector2, Tower> towers = new Dictionary<Vector2, Tower>();

	public List<Group> groups = new List<Group>();
	public List<Formation> formations = new List<Formation>();
	public List<Formation> potentials = new List<Formation>();
	
	public Gradient colorGradient = new Gradient();
	public Vector2[] relatives = {new Vector2(1,-0.5f), new Vector2(1,0.5f), new Vector2(0,1), new Vector2(-1,0.5f), new Vector2(-1,-0.5f), new Vector2(0,-1)};

	void Awake() {
		game = GetComponentInParent<GameManager>();
		CreateGradient();
	}
	
	public void UpdateMap() {
		LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();
		foreach (var line in lines) {
			Destroy(line.gameObject);
		}
		Collection.ResetLinks();

		foreach (var formation in formations) {
			formation.UpdateCollection();
			formation.DrawFlags();
		}
		foreach (var group in groups) {
			group.UpdateCollection();
		}
		UpdateOffenseMap();
		DrawRings();
		ColorPieces();
	}

	public void CreateBoard(int boardSize) {
		int m = boardSize;
		int M = 2*m-1;
		for (int x=0; x<M; x++) {
			float d = Mathf.Min(x, M-x-1);
			int s = (int)Mathf.Ceil((m - 1) / 2) - (int)Mathf.Ceil(d / 2);
			int e = (int)Mathf.Ceil((m - 1) / 2) + (int)Mathf.Ceil((d - 1) / 2) + m - 1;
			for (int y=s; y<=e; y++) {				
				float yOffset = x % 2 == 1 ? 0.5f : 0f;
				float yOff = y + yOffset;
				Vector2 coords = new Vector2(x, yOff);

				int h;
				float r = Mathf.Min(yOff, M - yOff - 1);
				if (d < (M / 3f) || d < m - r) {
					h = (int)r * 10 / M;
				} else {
					h = (int)r * -5 / M;
				}

				CreateTile(coords, h);

				int startingRows = boardSize / 3 + 1;
				if (y < s+d-m+startingRows || y > e-d+m-startingRows) {
					string team = y < s+d-m+startingRows ? "white" : "black";
					CreatePiece(coords, team);
				}

				if (y == m - 1) {
					if (new List<int>() {M * 1/6, M * 3/6, M * 5/6}.Contains(x)) {
						CreateTower(coords);
					}
				}
			}
		}
		UpdateOffenseMap();
	}

	public void CheckTowers(string team) {
		foreach (var tower in towers.Values) {
			if (tower.IsCaptured(team)) {
				tower.AddPieces(team);
			}
		}
	}

	void CreateGradient() {
		GradientColorKey[] colors = {new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.gray, 0.5f), new GradientColorKey(Color.green, 1.0f)};
		GradientAlphaKey[] alphas = {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f)};
		colorGradient.SetKeys(colors, alphas);
	}

	public bool NotAllied(Vector2 coords, string team) {
		return tiles.ContainsKey(coords) && (!pieces.ContainsKey(coords) || pieces[coords].team != team) && !towers.ContainsKey(coords);
	}
}
