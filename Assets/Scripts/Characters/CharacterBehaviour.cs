using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour {
	private bool isSelected = false;
	private bool isMoving = false;
	private string characterType;
	private Animator animator;
	private GameObject character;
	private SpriteRenderer spriteRenderer;
	private int speed = 2;
	private List<Point<int>> path = new List<Point<int>>();
	private Vector3 targetPos = new Vector3();
	private Vector3 lastPos;
	private int currentWayPoint = 0;

	// Use this for initialization
	void Start () {
		characterType = name;
		character = gameObject;
		animator = character.GetComponent<Animator> ();
		spriteRenderer = character.GetComponent<SpriteRenderer> ();
		lastPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		checkIfMoving ();
		moveCharacter ();
		playAnimation ();
	}

	private void checkIfMoving() {
		Vector3 curPos = transform.position;
		if (curPos != lastPos) {
			isMoving = true;
		} else {
			isMoving = false;
		}
		lastPos = curPos;
	}

	private void playAnimation () {
		Vector3 transpos = transform.position;
		if (isMoving) {
			// set facing
			if (transpos.x > targetPos.x) {
				spriteRenderer.flipX = true;
			} else {
				spriteRenderer.flipX = false;
			}
			// play animation
			if (transpos.y > targetPos.y) {
				animator.Play ("Citizen1R");
			} else {
				animator.Play ("Citizen1RUp");
			}
		} else {
			animator.Play ("");
		}
	}

	private void moveCharacter () {
		if (Input.GetMouseButtonDown (0)) {
		//if (false) {
			currentWayPoint = 0;
			targetPos = new Vector3 ();
			Vector3 mousePosition = Input.mousePosition;
			Vector3 startV = character.GetComponent<Renderer> ().transform.position;
			//Debug.Log ("-------------------------start-------------------------");
			//Debug.Log ("startV: " + startV.x + " " + startV.y);
			Vector3 iso_pt = Camera.main.ScreenToWorldPoint (mousePosition);
			//Debug.Log ("start: " + iso_pt.x + " " + iso_pt.y);
			Point<int> start = Point<int>.fromIsometricStart (new Point<float>(startV.x, startV.y));
			//Debug.Log ("start_character: " + start.x + " " + start.y);
			//Debug.Log ("-------------------------target-------------------------");
			Point<int> target = Point<int>.fromScreen(mousePosition);


			Point<float> target2iso = Point<float>.toIsometric (target);
			//Debug.Log ("Start back to iso: " +back2iso.x + " " + back2iso.y);
			//Debug.Log ("target: " + target.x + " " + target.y);
			//Debug.Log ("target2iso: " + target2iso.x + " " + target2iso.y);


			Pathfinding pathFinder = new Pathfinding ();
			path = pathFinder.pathFinding(start, target);
			//Debug.Log ("path length: " + path.Count);
		}

		if (currentWayPoint < this.path.Count) {
			if(targetPos == new Vector3()) {
				//Debug.Log ("currentWayPoint: " + currentWayPoint);
				Point<float> target = Point<float>.toIsometric (path [currentWayPoint]);
				targetPos = new Vector3 (target.x, target.y, 0);
			}
			walk ();
		}

	}

	private void walk() {
		transform.position = Vector3.MoveTowards (transform.position, targetPos, speed * Time.deltaTime);

		if (transform.position == targetPos) {
			currentWayPoint++;
			//Debug.Log ("path: " + this.path.Count);
			//Debug.Log ("path [currentWayPoint]: " + path [currentWayPoint].x + " " + path [currentWayPoint].y);
			if (currentWayPoint < this.path.Count) {
				Point<float> newTarget = Point<float>.toIsometric (path [currentWayPoint]);
				//Debug.Log ("newTarget: " + newTarget.x + " " + newTarget.y);
				//Debug.Log ("newTarget: " + transform.position.x + " " + transform.position.y);
				targetPos = new Vector3 (newTarget.x, newTarget.y, 0);
			}
		}
	}
}
