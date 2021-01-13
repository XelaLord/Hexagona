using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour {

	public HexGrid map;
	public GamePiece[] pieces;
	public Color color;
	static List<(GamePiece, GamePiece)> links = new List<(GamePiece, GamePiece)>();

	void Awake() {
		map = GetComponentInParent<HexGrid>();
	}

	public static void ResetLinks() {
		links.Clear();
	}

	public void UpdateCollection() {
		transform.position = pieces[0].transform.position;		
		DrawLinks(color);
	}

	public void DrawLinks(Color color) {
		foreach (var piece in pieces) {
			List<GamePiece> neighbors = piece.GetNeighbors();

			foreach (var neighbor in neighbors) {
				bool check = System.Array.IndexOf(pieces, neighbor) != -1;
				if (check) {
					if (!links.Contains((piece, neighbor)) && !links.Contains((neighbor, piece))) {
						DrawLink(piece, neighbor, color);
						links.Add((piece, neighbor));	
					}
				}
			}
		}
	}

	void DrawLink(GamePiece start, GamePiece end, Color color) {
		GameObject lineObject = new GameObject("Link", typeof(LineRenderer));
		lineObject.transform.parent = this.transform;
		LineRenderer line = lineObject.GetComponent<LineRenderer>();
		Vector3[] positions = {start.transform.position, end.transform.position};
		line.SetPositions(positions);
		line.useWorldSpace = false;
		line.startWidth = color == Color.grey ? 0.08f : 0.1f;
		line.endWidth = color == Color.grey ? 0.08f : 0.1f;
		line.material = new Material(Shader.Find("Standard"));
		line.GetComponent<Renderer>().material.color = color;
		line.generateLightingData = true;
		line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}

	void OnDestroy() {
		foreach (var piece in pieces) {
			List<GamePiece> neighbors = piece.GetNeighbors();

			foreach (var neighbor in neighbors) {
				bool check = System.Array.IndexOf(pieces, neighbor) != -1;
				if (check) {
					links.Remove((piece, neighbor));
					links.Remove((neighbor, piece));
				}
			}
		}
	}
}