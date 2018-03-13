using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Structure {

	// Use this for initialization
	void Start () {
		base.Start ();
		woodCost = 10;
		stoneCost = 10;
		maxHealthPoints = 1000;
		attackPoint = 10;
		healthPoints = 0;
		level = 0;
		isBuilt = false;
		isBuildingInProgress = false;

	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	public override void UnitReturned (Character unit, string resource, int amount) {
		player.IncreaseResource (resource, amount);
		if (Map.resourceData [unit.targetPoint.x, unit.targetPoint.y] != null && Map.resourceData [unit.targetPoint.x, unit.targetPoint.y].script.amountLeft > 0) {
			//if (Map.resourceData [unit.targetPoint.x, unit.targetPoint.y].script.amountLeft > 0) {
				unit.SendMessage ("AcquireTarget", unit.targetPoint);
			//}
		} else {
			unit.SendMessage ("AcquireTarget", Pathfinding.FindNearestType (Point<int>.FromIsometric (new Point<float> (entranceExit.x, entranceExit.y)), resource));
		}
	}
}
