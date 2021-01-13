using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Camera cam;
	public bool inputActive = true;
	public bool cameraActive = true;

	public float movementSpeed;
	public float scrollSensitivity;
	float zoomLevel = 15f;

	void Awake() {
		cam = GetComponentInChildren<Camera>();
	}

	void Update() {
		if (cameraActive && inputActive && !PauseMenu.isPaused) {
			HandleMovementInput();
		}
	}

	void HandleMovementInput() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		transform.Translate(new Vector3(horizontal, 0, vertical) * movementSpeed * cam.transform.position.y);

		SetZoomLevel(zoomLevel - Input.mouseScrollDelta.y * scrollSensitivity);
	}

	public void SetZoomLevel(float zoom) {
		zoomLevel = Mathf.Max(0, zoom);
		cam.transform.localPosition = Vector3.zero;
		cam.transform.Translate(0, 0, -zoomLevel);
	}

	public void SetActive(bool cameraActive) {
		cam.enabled = cameraActive;
		this.cameraActive = cameraActive;
	}
}
