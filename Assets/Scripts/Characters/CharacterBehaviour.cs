using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour {
	private bool isMoving = false;
	private string characterType;
	private Animator animator;
	private GameObject character;
	private SpriteRenderer spriteRenderer;
	private int speed = 2;
	private List<Point<int>> path = new List<Point<int>>();
	private Vector3 targetPos = Vector3.zero;
	private Vector3 lastPos;
	Pathfinding pathFinder = new Pathfinding ();
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

	private void acquireTarget(Vector3 targetPosition) {
		currentWayPoint = 0;
		targetPos = Vector3.zero;
		Vector3 startV = character.GetComponent<Renderer> ().transform.position;
		Point<int> start = Point<int>.fromIsometricStart (new Point<float>(startV.x, startV.y));
		Point<int> target = Point<int>.fromScreen(targetPosition);
		path = pathFinder.pathFinding(start, target);
	}

	private void moveCharacter () {
		if (currentWayPoint < this.path.Count) {
			if(targetPos == Vector3.zero) {
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

	private void checkIfInSelecionRectangle(Vector4 bounds) {
		bool isXWithin = false;
		bool isYWithin = false;
		if (bounds.x - bounds.z > 0) {
			if (lastPos.x >= bounds.z && lastPos.x <= bounds.x) {
				isXWithin = true;
				//Debug.Log ("1");
			}
		} else {
			if (lastPos.x >= bounds.x && lastPos.x <= bounds.z) {
				isXWithin = true;
				//Debug.Log ("2");
			}
		}

		if (bounds.y - bounds.w > 0) {
			if (lastPos.y >= bounds.w && lastPos.y <= bounds.y) {
				isYWithin = true;
			}
		} else {
			if (lastPos.y >= bounds.y && lastPos.y <= bounds.w) {
				isYWithin = true;
			}
		}

		if (isXWithin && isYWithin) {
			Camera.main.BroadcastMessage ("addSelectedGameObject", character);
		}
	}
}
