using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HexGrid {

	public void DeleteBoard() {
		foreach (var tile in tiles.Values) {
			Destroy(tile);
		}
		foreach (var piece in pieceList) {
			Destroy(piece);
		}
		foreach (var piece in pieces.Values) {
			Destroy(piece);
		}
		foreach (var tower in towers.Values) {
			Destroy(tower);
		}
		foreach (var group in groups) {
			Destroy(group);
		}
		foreach (var formation in formations) {
			Destroy(formation);
		}
		foreach (var potential in potentials) {
			Destroy(potential);
		}

		tiles.Clear();
		pieces.Clear();
		pieceList.Clear();
		towers.Clear();
		groups.Clear();
		formations.Clear();
		potentials.Clear();
	}

	public void CreateTile(Vector2 coords, int h) {
		if (!tiles.ContainsKey(coords)) {
			GameObject newTile = Instantiate(hexPrefab, transform.Find("Tiles"));
			Hex hex = newTile.GetComponent<Hex>();
			tiles.Add(coords, hex);
			tiles[coords].Initialise(coords, h);
		} else {
			tiles[coords].SetElevation(h);
		}
	}

	public void CreatePiece(Vector2 coords, string team) {
		if (!pieces.ContainsKey(coords)) {
			GameObject newpiece = Instantiate(piecePrefab, transform.Find("Game Pieces"));
			GamePiece piece = newpiece.GetComponent<GamePiece>();
			//pieces.Add(coords, piece);
			//pieceList.Add(piece);
			piece.Initialise(coords, team);
		} else {
			pieces[coords].team = team;
		}
	}	

	public void CreateTower(Vector2 coords) {
		GameObject newTower = Instantiate(towerPrefab, transform.Find("Towers"));
		Tower tower = newTower.GetComponent<Tower>();
		towers.Add(coords, tower);
		tower.Initialise(coords);
	}

	public void CreateGroup(GamePiece[] pieces) {
		GameObject newGroup = new GameObject("Group " + groups.Count);
		newGroup.transform.parent = transform.Find("Groups");

		Group group = newGroup.AddComponent<Group>();
		group.Initialise(pieces);
		groups.Add(group);
		foreach (var piece in pieces) {
			piece.group = group;
		}
	}

	public void DeleteGroup(Group group) {
		groups.Remove(group);
		Destroy(group.gameObject);
	}

	public void CreateFormation(GamePiece[] pieces, Shape shape) {
		GameObject newFormation = new GameObject("Formation " + formations.Count);
		newFormation.transform.SetParent(transform.Find("Engaged Formations"));

		Formation formation = newFormation.AddComponent<Formation>();
		formation.Initialise(pieces, shape);
		formations.Add(formation);
		foreach (var piece in pieces) {
			piece.formations.Add(formation);
		}
	}

	public void DeleteFormation(Formation formation) {
		int sign = formation.pieces[0].team == "white" ? 1 : -1;
		foreach (var item in formation.shape.terrain) {
			Vector2 coords = sign * item.Key + formation.pieces[0].coords;
			if (tiles.ContainsKey(coords)) {
				Hex tile = tiles[coords];
				tile.SetElevation(tile.elevation + item.Value);
			}
		}
		
		formations.Remove(formation);
		Destroy(formation.gameObject);
		
	}

	public void CreatePotential(GamePiece[] pieces, Shape shape) {
		GameObject newPotential = new GameObject("Potential " + potentials.Count);
		newPotential.transform.parent = transform.Find("Potential Formations");

		Formation potential = newPotential.AddComponent<Formation>();
		potential.Initialise(pieces, shape, true);
		potentials.Add(potential);
		foreach (var piece in pieces) {
			piece.potentials.Add(potential);
		}
	}
}