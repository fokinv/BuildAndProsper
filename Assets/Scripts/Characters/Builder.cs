using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Character {
	private bool isBuilding = false;
	private bool isOnTheWay = false;
	private bool isMining = false;
	private bool isReturningWithResource = false;

	private int maxCarriedResourses;
	private int carriedResourseAmount;
	private string carriedResourceName;
	// Use this for initialization
	void Start () {
		base.Start ();
		healthPoints = 100;
		damagePerSecond = 10;
		walkingSpeed = 5;
		actionSpeed = 5.0f;
		maxCarriedResourses = 5;
		carriedResourseAmount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		if (currentWayPoint == path.Count && isOnTheWay) {
			if (isBuilding) {
				Map.buildingData[targetPoint.x, targetPoint.y].tile.SendMessage ("StartBuilding", transform);
				isOnTheWay = false;
			} else if (isMining && !isReturningWithResource) {
				Debug.Log ("Trying to cut down: " + targetPoint.x + " " + targetPoint.y + " Tile: " + Map.resourceData [targetPoint.x, targetPoint.y].tile);
				carriedResourceName = Map.resourceData [targetPoint.x, targetPoint.y].tile.name;
				//Map.resourceData[targetPoint.x, targetPoint.y].tile.SendMessage ("StartMining", transform);
				Map.resourceData[targetPoint.x, targetPoint.y].script.StartMining(transform);
				isOnTheWay = false;
			} else if (isReturningWithResource) {
				isReturningWithResource = false;
				isOnTheWay = false;
				assignedStructure.script.UnitReturned (this, carriedResourceName, carriedResourseAmount);
				carriedResourseAmount = 0;
				carriedResourceName = "";
			}
		}
	}

	private void PlayAnimation() {
		base.PlayAnimation();
		if (isBuilding) {
			//animator.Play ("Building");
		} else if (isMining) {
			//animator.Play ("Mining");
		}
	}

	private void BuildingIsCompleted() {
		isBuilding = false;
		isOnTheWay = false;
	}

	private void AcquireTarget(Point<int> target) {
		base.AcquireTarget (target);
		if (!isReturningWithResource) {
			if (Map.buildingData [target.x, target.y] != null) {
				isOnTheWay = true;
				targetPoint = target;
				isBuilding = true;
			} else if (Map.resourceData [target.x, target.y] != null) {
				isOnTheWay = true;
				targetPoint = target;
				isMining = true;
			}
		}
	}

	private void ReturnToBuilding () {
		if (assignedStructure == null) {
			Vector3 transPos = GetComponent<Renderer> ().transform.position;
			Point<int> start = Point<int>.FromIsometricStart (new Point<float>(transPos.x, transPos.y));
			Point<int> castlePoint = Pathfinding.FindNearestType(start, "Castle");
			Debug.Log ("castle: " + castlePoint.x + " " + castlePoint.y);
			assignedStructure = Map.buildingData[castlePoint.x, castlePoint.y];
		}
		Vector3 structurePos = assignedStructure.tile.GetComponent<Renderer> ().transform.position;
		Point<int> structurePoint = Point<int>.FromIsometricStart (new Point<float>(structurePos.x, structurePos.y));

		Structure structureScript = assignedStructure.script;
		Point<int> tempTarget = Point<int>.FromIsometric (new Point<float> (structureScript.entranceExit.x, structureScript.entranceExit.y));
		isOnTheWay = true;
		isReturningWithResource = true;
		AcquireTarget (tempTarget);
	}

	private void IncreaseCarriedResources(int amount) {
		carriedResourseAmount += amount;
		Debug.Log (carriedResourseAmount);
		if (carriedResourseAmount >= maxCarriedResourses) {
			if (Map.resourceData [targetPoint.x, targetPoint.y] != null && Map.resourceData [targetPoint.x, targetPoint.y].script.amountLeft > 0) {
				Map.resourceData [targetPoint.x, targetPoint.y].tile.SendMessage ("PauseMining", transform);
			} else {
				isMining = false;
			}
			ReturnToBuilding ();
		} else {
			if (Map.resourceData [targetPoint.x, targetPoint.y] == null || Map.resourceData [targetPoint.x, targetPoint.y].script.amountLeft == 0) {
				isMining = false;
				ReturnToBuilding ();
			}
		}
	}
}
