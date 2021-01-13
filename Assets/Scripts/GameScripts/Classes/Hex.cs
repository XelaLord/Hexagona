using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

	public Vector2 coords;
	public int elevation;
	float tileGapRatio = 1.05f;
	float yScale = 0.5f;
	HexGrid HexGrid;

	void Awake() {
		HexGrid = GetComponentInParent<HexGrid>();
	}

	public void Initialise(Vector2 coord, int h) {
		coords = coord;
		Vector2 pos = new Vector2(1.5f * coords.x, Mathf.Sqrt(3) * coords.y) * tileGapRatio;
		elevation = h;

		transform.position = new Vector3(pos.x, 0, pos.y);
		SetElevation(elevation);
		name = "Hex "+ coords.ToString();
	}

	public void SetElevation(int elevation) {
		elevation = (int)Mathf.Clamp(elevation, -5, 5);
		this.elevation = elevation;
		float time = elevation + 6;
		transform.localScale = new Vector3(1f, 1f, time * yScale / 2f);

		if (HexGrid.pieces.ContainsKey(coords)) {
			HexGrid.pieces[coords].UpdatePosition();
		}
		GetComponent<Renderer>().material.color = HexGrid.colorGradient.Evaluate((time - 1) / 10f);
	}

	public Vector3 GetTop() {
		return new Vector3(transform.position.x, (elevation + 6) * yScale, transform.position.z);
	}

	public void DrawRings(int offense) {
		Color color = offense > 0 ? Color.red : Color.blue;

		for (int i=0; i<transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}

		for (int i = 0; i<Mathf.Abs(offense); i++) {
			GameObject ring = new GameObject("Ring", typeof(LineRenderer), typeof(Ring));
			ring.transform.parent = transform;
			ring.transform.position = GetTop();
			ring.transform.Translate((i + 1) * new Vector3(0, yScale, 0));
			ring.GetComponent<Ring>().DrawRing(0.7f, 0.05f, color);	
		}
	}

	void OnDestroy() {
		Destroy(gameObject);
	}
}