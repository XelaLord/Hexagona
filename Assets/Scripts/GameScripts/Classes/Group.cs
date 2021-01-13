using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : Collection {

	public void Initialise(GamePiece[] pieces) {
		this.pieces = pieces;
		transform.position = pieces[0].transform.position;
		color = pieces[0].team=="white" ? Color.white : Color.black;

		UpdateCollection();
	}

	void OnDestroy() {
		foreach (var piece in pieces) {
			piece.group = null;
		}
		map.groups.Remove(this);
		Destroy(gameObject);
	}
}

[System.Serializable]
public class GroupData {

	public Vector2[] pieces;

	public GroupData(Group group) {
		pieces = group.pieces.Select(piece => piece.coords).ToArray();
	}
}