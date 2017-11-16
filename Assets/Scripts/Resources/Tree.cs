using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Resource {
	private Animator animator;
	private bool isCutDown = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		playAnimation ();
		
	}
	
	// Update is called once per frame
	void Update () {
		playAnimation ();
	}

	private void playAnimation () {
		Vector3 transpos = transform.position;
		if (isCutDown) {
			animator.Play ("Tree1CutDown");
		} else {
			animator.Play ("Tree1Idle");
		}
	}
}
