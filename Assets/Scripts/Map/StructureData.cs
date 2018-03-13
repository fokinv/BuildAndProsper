using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureData {
	public StructureType structureType { get; set; }
	public Transform tile { get; set; }
	public Structure script { get; set; }

	public enum StructureType {
		Castle,
		Chapel,
		Woodcutter
	}

	public StructureData(StructureType tileType, Transform tile = null) {
		this.structureType = tileType;
		this.tile = tile;
		if (tile.name.Contains ("Castle")) {
			script = tile.GetComponent<Castle> ();
		} /*else if (tile.name.Contains ("Stone")) {
			script = tile.GetComponent<Stone> ();
		}*/
	}
}
