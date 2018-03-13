using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Resource {
	private Animator animator;
	private bool isCutDown = false;

	// Use this for initialization
	void Start () {
		maxMiners = 1;
		amountLeft = 5;
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		PlayAnimation ();
		if (isUnderMining) {
			Mine ();
		}
	}

	private void PlayAnimation () {
		//Vector3 transpos = transform.position;
		if (isCutDown) {
			isUnderMining = false;
			animator.Play ("Tree1CutDown");
			bool finishedAnimation = animator.GetCurrentAnimatorStateInfo (0).IsName("Tree1CutDown") && animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1.0f;
			if (finishedAnimation) {
				Point<int> coords = Point<int>.FromIsometricStart (new Point<float> (transform.position.x, transform.position.y));
				//Debug.Log ("Tree cut down: " + coords.x + " " + coords.y);
				Map.resourceData[coords.x, coords.y] = null;
				Map.mapData [coords.x, coords.y].isWalkable = true;
				Destroy (gameObject);
			}
		}
	}

	private void Mine() {
		base.Mine ();
		if (amountLeft <= 0 && !isCutDown) {
			isCutDown = true;
		}
	}
}
