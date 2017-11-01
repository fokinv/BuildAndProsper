using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {
	
	public int prefabID { get; set; }
	public bool isWalkable { get; set; }
	public TileType tileType { get; set; }
	public Transform tile { get; set; }

	public enum TileType {
		Ground,
		Hill,
		Lake,
		Building,
		Resource,
	}

	public TileData() {
		prefabID = Random.Range (1, 20);
		isWalkable = true;
		tileType = TileType.Ground;
		tile = null;
	}

	public TileData(int prefabID, bool isWalkable, TileType tileType, Transform tile = null) {
		this.prefabID = prefabID;
		this.isWalkable = isWalkable;
		this.tileType = tileType;
		this.tile = tile;
	}

	public void changeTile(int prefabID, bool isWalkable, TileType tileType, Transform tile = null) {
		this.prefabID = prefabID;
		this.isWalkable = isWalkable;
		this.tileType = tileType;
		this.tile = tile;
	}

	public void initTile(Transform tile) {
		this.tile = tile;
	}

	public bool isSameEnvironment(TileType type) {
		return tileType == type;
	}
}
