using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave {

	public Vector2[] terrainKeys;
	public int[] terrainValues;
	public Vector2[] pieceKeys;
	public bool[] pieceTeams;
	public Vector2[] towerKeys;

	public bool turnState;
	public int turnEnergy;
	public int currentEnergy;

	public ShapeData[] whiteShapes;
	public ShapeData[] blackShapes;
	public FormationData[] formations;
	public GroupData[] groups;

	public GameSave(GameManager game) {
		turnState = game.turnState == "white";
		turnEnergy = game.turnEnergy;
		currentEnergy = game.currentEnergy;

		terrainKeys = game.map.tiles.Keys.ToArray();
		terrainValues = game.map.tiles.Values.Select(tile => tile.elevation).ToArray();

		pieceKeys = game.map.pieces.Keys.ToArray();
		pieceTeams = game.map.pieces.Values.Select(piece => piece.team == "white").ToArray();

		towerKeys = game.map.towers.Keys.ToArray();

		whiteShapes = game.whiteShapes.Select(shape => new ShapeData(shape)).ToArray();
		blackShapes = game.blackShapes.Select(shape => new ShapeData(shape)).ToArray();
		formations = game.map.formations.Select(formation => new FormationData(formation)).ToArray();
		groups = game.map.groups.Select(group => new GroupData(group)).ToArray();
	}
}
