using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Structure {

	// Use this for initialization
	void Start () {
		base.Start ();
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
}
