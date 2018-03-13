using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	protected int healthPoints;
	protected int damagePerSecond;
	protected int walkingSpeed;
	protected int level;
	public float actionSpeed { get; set; } // How many actions it does in a second

	private bool isMoving = false;
	protected Animator animator;
	protected SpriteRenderer spriteRenderer;

	protected List<Point<int>> path = new List<Point<int>>();
	private Vector3 targetPos = Vector3.zero;
	private Vector3 lastPos;
	private Pathfinding pathFinder = new Pathfinding ();
	protected int currentWayPoint = 0;
	public Point<int> targetPoint { get; set; }
	protected StructureData assignedStructure = null;

	public Player player { get; set; }

	// Use this for initialization
	protected void Start () {
		Player pl = Camera.main.GetComponent (typeof(Player)) as Player;
		player = pl;
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		lastPos = transform.position;
		level = 0;
	}

	protected void Update () {
		CheckIfMoving ();
		CheckSortingOrder ();
		MoveCharacter ();
		PlayAnimation ();
	}

	private void CheckIfMoving() {
		Vector3 curPos = transform.position;
		if (curPos != lastPos) {
			isMoving = true;
		} else {
			isMoving = false;
		}
		lastPos = curPos;
	}

	private void CheckSortingOrder () {
		Point<int> currentPosition = Point<int>.FromIsometricStart (new Point<float> (lastPos.x, lastPos.y));
		int currentLayer = (Map.mapSizeX + Map.mapSizeY) - (currentPosition.x + currentPosition.y);
		spriteRenderer.sortingOrder = currentLayer;
	}

	protected void PlayAnimation () {
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

	protected void AcquireTarget(Point<int> target) {
		currentWayPoint = 0;
		targetPos = Vector3.zero;
		Vector3 startV = GetComponent<Renderer> ().transform.position;
		Point<int> start = Point<int>.FromIsometricStart (new Point<float>(startV.x, startV.y));
		//Point<int> target = Point<int>.fromScreen(targetPosition);
		path = pathFinder.PathFinding(start, target);
	}

	private void MoveCharacter () {
		if (currentWayPoint < path.Count) {
			if(targetPos == Vector3.zero) {
				Point<float> target = Point<float>.ToIsometric (path [currentWayPoint]);
				targetPos = new Vector3 (target.x, target.y, 0);
			}
			Walk ();
		}

	}

	private void Walk() {
		transform.position = Vector3.MoveTowards (transform.position, targetPos, walkingSpeed * Time.deltaTime);

		if (transform.position == targetPos) {
			currentWayPoint++;
			if (currentWayPoint < this.path.Count) {
				Point<float> newTarget = Point<float>.ToIsometric (path [currentWayPoint]);
				targetPos = new Vector3 (newTarget.x, newTarget.y, 0);
			}
		}
	}

	private void CheckIfInSelecionRectangle(Vector4 bounds) {
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
			Camera.main.BroadcastMessage ("AddSelectedGameObject", transform);
		}
	}

	private void ShowMenu() {
		GameObject canvas = GameObject.Find ("Canvas");
		canvas.SendMessage ("Selected", transform);
	}
		
	protected void UnSelect() {
		
	}
}
