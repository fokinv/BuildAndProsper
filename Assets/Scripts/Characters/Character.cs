using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	protected int healthPoints;
	protected int walkingSpeed;
	protected int level;
	protected int actionSpeed;

	private bool isMoving = false;
	private Animator animator;
	private SpriteRenderer spriteRenderer;

	protected List<Point<int>> path = new List<Point<int>>();
	private Vector3 targetPos = Vector3.zero;
	private Vector3 lastPos;
	private Pathfinding pathFinder = new Pathfinding ();
	protected int currentWayPoint = 0;
	protected Point<int> targetPoint = new Point<int> ();

	// Use this for initialization
	protected void Start () {
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lastPos = transform.position;
		level = 0;
	}

	protected void Update () {
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

	protected void acquireTarget(Point<int> target) {
		currentWayPoint = 0;
		targetPos = Vector3.zero;
		Vector3 startV = GetComponent<Renderer> ().transform.position;
		Point<int> start = Point<int>.fromIsometricStart (new Point<float>(startV.x, startV.y));
		//Point<int> target = Point<int>.fromScreen(targetPosition);
		path = pathFinder.pathFinding(start, target);
	}

	private void moveCharacter () {
		if (currentWayPoint < path.Count) {
			if(targetPos == Vector3.zero) {
				Point<float> target = Point<float>.toIsometric (path [currentWayPoint]);
				targetPos = new Vector3 (target.x, target.y, 0);
			}
			walk ();
		}

	}

	private void walk() {
		transform.position = Vector3.MoveTowards (transform.position, targetPos, walkingSpeed * Time.deltaTime);

		if (transform.position == targetPos) {
			currentWayPoint++;
			if (currentWayPoint < this.path.Count) {
				Point<float> newTarget = Point<float>.toIsometric (path [currentWayPoint]);
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
			}
		} else {
			if (lastPos.x >= bounds.x && lastPos.x <= bounds.z) {
				isXWithin = true;
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
			Camera.main.BroadcastMessage ("addSelectedGameObject", transform);
		}
	}

	private void showMenu() {
		GameObject canvas = GameObject.Find ("Canvas");
		canvas.SendMessage ("selected", transform);
	}

	protected void unSelect() {
		
	}
}
