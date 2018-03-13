using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceData {
	public ResourceType resourceType { get; set; }
	public Transform tile { get; set; }
	public Resource script { get; set; }

	public enum ResourceType {
		Wood,
		Stone,
		Food
	}

	public ResourceData(ResourceType tileType, Transform tile = null) {
		this.resourceType = tileType;
		this.tile = tile;
		InitTile (tile);
	}

	public void InitTile(Transform tile) {
		if (tile) {
			this.tile = tile;
			if (tile.name.Contains ("Tree")) {
				script = tile.GetComponent<Tree> ();
			} else if (tile.name.Contains ("Stone")) {
				script = tile.GetComponent<Stone> ();
			}
		}
	}
}
