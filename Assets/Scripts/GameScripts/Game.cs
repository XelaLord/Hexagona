using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game {

	static string[] commonShapeNames;
	static string[] whiteShapeNames;
	static string[] blackShapeNames;

	static void Save() {
		ShapeList list = new ShapeList();	
		list.commonShapeNames = commonShapeNames;
		list.whiteShapeNames = whiteShapeNames;
		list.blackShapeNames = blackShapeNames;

		string path = Application.dataPath + "/Resources/Shapes/Master.txt";
		new FileInfo(path).Directory.Create();
		string json = JsonUtility.ToJson(list);
		File.WriteAllText(path, json);
	}

	static void Load() {
		string path = Application.dataPath + "/Resources/Shapes/Master.txt";

		if (File.Exists(path)) {
			string json = File.ReadAllText(path);
			ShapeList list = JsonUtility.FromJson<ShapeList>(json);
			commonShapeNames = list.commonShapeNames;
			whiteShapeNames = list.whiteShapeNames;
			blackShapeNames = list.blackShapeNames;
		}
	}

	public static string[] GetWhiteShapes() {
		Load();
		return commonShapeNames.Concat(whiteShapeNames).ToArray();
	}

	public static string[] GetBlackShapes() {
		Load();
		return commonShapeNames.Concat(blackShapeNames).ToArray();
	}
}

public struct ShapeList {

	public string[] commonShapeNames;
	public string[] whiteShapeNames;
	public string[] blackShapeNames;
}