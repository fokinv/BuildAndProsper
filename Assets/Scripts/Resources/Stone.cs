using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Resource {

	// Use this for initialization
	void Start () {
		maxMiners = 3;
	}
	
	// Update is called once per frame
	void Update () {
		if (isUnderMining) {
			Mine ();
		}
	}

	private void Mine() {
		base.Mine ();
		Debug.Log (amountLeft);
		if (amountLeft <= 0) {
			Point<int> coords = Point<int>.FromIsometricStart (new Point<float> (transform.position.x, transform.position.y));
			Map.resourceData[coords.x, coords.y] = null;
			Map.mapData [coords.x, coords.y].isWalkable = true;
			Destroy (gameObject);
		}
	}
}
