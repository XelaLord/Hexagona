using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCamera : MonoBehaviour {

	public EditorManager manager;
	public EditorUI UI;
	public Camera firstCamera;
	bool active = false;
	bool picture = false;
	string path;

	void Update() {
		if (active) {
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");
			transform.Translate(new Vector3(horizontal, vertical, 0) * 0.05f);

			GetComponent<Camera>().orthographicSize += -0.25f * Input.mouseScrollDelta.y;

			if (Input.GetKeyDown(KeyCode.Return)) {
				picture = true;
			}
		}
	}

	public void TakePicture(Shape shape) {
		path = Application.dataPath + "/Resources/Shapes/" + EditorManager.shape.shapeName + ".png";
		FindCenter(EditorManager.shape);		

		firstCamera.enabled = false;
		UI.gameObject.SetActive(false);
		active = true;
	}

	void FindCenter(Shape shape) {
		int length = shape.positive.Length + shape.negative.Length + shape.offense.Count + shape.defense.Count + shape.terrain.Count;
		Vector2[] allCoords = new Vector2[length];
		shape.positive.CopyTo(allCoords, 0);
		shape.negative.CopyTo(allCoords, shape.positive.Length);
		shape.offense.Keys.CopyTo(allCoords, shape.positive.Length + shape.negative.Length);
		shape.defense.Keys.CopyTo(allCoords, length - shape.defense.Count - shape.terrain.Count);
		shape.terrain.Keys.CopyTo(allCoords, length - shape.terrain.Count);

		Vector2 min = Vector2.positiveInfinity;
		Vector2 max = Vector2.negativeInfinity;
		foreach (var coords in allCoords) {
			if (coords.x < min.x) {
				min.x = coords.x;
			} else if (coords.x > max.x) {
				max.x = coords.x;
			}

			if (coords.y < min.y) {
				min.y = coords.y;
			} else if (coords.x > max.y) {
				max.y = coords.y;
			}
		}

		Vector2 center = (max + min) / 2f;
		float scale = (max - min).magnitude;
		GetComponent<Camera>().orthographicSize = scale * 0.75f;
		transform.position = new Vector3(0.75f * center.x, 5, Mathf.Sqrt(3)/2f * center.y);
	}

	void OnPostRender() {
		if (picture) {
			Texture2D texture = new Texture2D(Screen.height, Screen.height);
			int offset = (Screen.width - Screen.height) / 2;
			Rect rect = new Rect(offset, 0, Screen.height, Screen.height);
			texture.ReadPixels(rect, 0, 0, false);
			Color[] pixels = texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				if (pixels[i] == new Color(1, 0, 1, 1)) {
					pixels[i] = new Color(0, 0, 0, 0);
				}
			}
			texture.SetPixels(pixels);
			texture.Apply();
			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(path, bytes);

			firstCamera.enabled = true;
			UI.gameObject.SetActive(true);
			picture = false;
			active = false;
		}
	}
}