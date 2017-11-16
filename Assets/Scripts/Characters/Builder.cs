using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Character {
	private bool isBuilding = false;
	private bool isBuildingStarted = false;
	// Use this for initialization
	void Start () {
		base.Start ();
		healthPoints = 100;
		walkingSpeed = 5;
		actionSpeed = 1;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		if (isBuilding && currentWayPoint == path.Count && !isBuildingStarted) {
			Map.buildingData[targetPoint.x, targetPoint.y].tile.SendMessage ("startBuilding", transform);
			isBuildingStarted = true;
		}
	}

	private void buildingIsCompleted() {
		isBuilding = false;
		isBuildingStarted = false;
	}

	private void acquireTarget(Point<int> target) {
		if (!isBuilding) {
			base.acquireTarget (target);
			if (Map.buildingData [target.x, target.y] != null) {
				targetPoint = target;
				isBuilding = true;
			}
		}
	}
}
