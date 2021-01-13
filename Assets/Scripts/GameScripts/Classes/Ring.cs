using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Ring : MonoBehaviour {
	
	public void DrawRing(float radius, float width, Color color) {
		LineRenderer line = GetComponent<LineRenderer>();
		Vector3[] positions = new Vector3[24];
		for (int i = 0; i<24; i++) {
			float angle = i / 12f * Mathf.PI;
			positions[i] = radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
		}

		line.positionCount = 24;
		line.SetPositions(positions);
		line.useWorldSpace = false;
		line.startWidth = width;
		line.endWidth = width;
		line.material = new Material(Shader.Find("Standard"));
		line.GetComponent<Renderer>().material.color = color;
		line.generateLightingData = true;
		line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		line.loop = true;
	}
}
