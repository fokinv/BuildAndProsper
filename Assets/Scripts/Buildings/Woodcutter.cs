using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woodcutter : Structure {

	// Use this for initialization
	void Start () {
		base.Start ();
		woodCost = 10;
		stoneCost = 10;
		maxHealthPoints = 300;
		attackPoint = 0;
		healthPoints = 0;
		level = 0;
		isBuilt = false;
		isBuildingInProgress = false;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}
}
