using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape {
	
	public string shapeName;
	public int creationEnergy;
	public int destructionEnergy;
	public Vector2[] positive;
	public Vector2[] negative;
	public Dictionary<Vector2, int> terrain = new Dictionary<Vector2, int>();
	public Dictionary<Vector2, int> offense = new Dictionary<Vector2, int>();
	public Dictionary<Vector2, int> defense = new Dictionary<Vector2, int>();
	public Color flagColor;

	public Shape() {
		positive = new Vector2[0];
		negative = new Vector2[0];
	}

	public Shape(ShapeData data) {
		this.shapeName = data.shapeName;
		this.creationEnergy = data.creationEnergy;
		this.destructionEnergy = data.destructionEnergy;
		this.positive = data.positive;
		this.negative = data.negative;
		this.flagColor = data.flagColor;

		int i = 0;
		foreach (var key in data.terrainKeys) {
			terrain.Add(key, data.terrainValues[i]);
			i++;
		}
		i = 0;
		foreach (var key in data.offenseKeys) {
			offense.Add(key, data.offenseValues[i]);
			i++;
		}
		i = 0;
		foreach (var key in data.defenseKeys) {
			defense.Add(key, data.defenseValues[i]);
			i++;
		}
	}

	public static void Save(Shape shape) {
		string path = Application.dataPath + "/Resources/Shapes/" + shape.shapeName + ".shp";
		string json = JsonUtility.ToJson(new ShapeData(shape));
		new FileInfo(path).Directory.Create();
		File.WriteAllText(path, json);
	}

	public static Shape Load(string shapeName) {
		string path = Application.dataPath + "/Resources/Shapes/" + shapeName + ".shp";
		if (File.Exists(path)) {
			string json = File.ReadAllText(path);
			ShapeData shapeData = JsonUtility.FromJson<ShapeData>(json);
			return new Shape(shapeData);
		} else {
			return null;
		}
	}
}

[System.Serializable]
public class ShapeData {

	public string shapeName;
	public int creationEnergy;
	public int destructionEnergy;
	public Vector2[] positive;
	public Vector2[] negative;
	public Vector2[] terrainKeys;
	public int[] terrainValues;
	public Vector2[] offenseKeys;
	public int[] offenseValues;
	public Vector2[] defenseKeys;
	public int[] defenseValues;
	public Color flagColor;

	public ShapeData(Shape shape) {
		this.shapeName = shape.shapeName;
		this.creationEnergy = shape.creationEnergy;
		this.destructionEnergy = shape.destructionEnergy;
		this.positive = shape.positive;
		this.negative = shape.negative;
		this.flagColor = shape.flagColor;

		this.terrainKeys = shape.terrain.Keys.ToArray();
		this.terrainValues = shape.terrain.Values.ToArray();
		this.offenseKeys = shape.offense.Keys.ToArray();
		this.offenseValues = shape.offense.Values.ToArray();
		this.defenseKeys = shape.defense.Keys.ToArray();
		this.defenseValues = shape.defense.Values.ToArray();
	}
}