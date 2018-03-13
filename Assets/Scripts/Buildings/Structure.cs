using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {
	public int maxHealthPoints { get; set; }
	public int healthPoints { get; set; }
	public int attackPoint { get; set; }
	public int level { get; set; }
	public int timeToGatherResource { get; set; }

	public bool isBuilt { get; set; }
	public bool isBuildingInProgress { get; set; }

	private float amountBuilt = 0.0f;
	public Vector4 originalColour { get; set; }

	// this will be defined by worker
	private float combinedBuildingSpeed = 0.0f;

	private Point<float> topRightPoint;
	private Point<int> flagPoint;
	private Transform left;
	private Transform right;
	public Vector3 entranceExit { get; set; }

	private GameObject characters;
	private Transform flagObject;

	private List<Transform> assignedUnits = new List<Transform> ();

	public Player player { get; set; }
	public int woodCost { get; set; }
	public int stoneCost { get; set; }
	//resources cost

	// Use this for initialization
	protected void Start () {
		characters = GameObject.Find ("CharactersObject");
		left = transform.Find ("left");
		right = transform.Find ("right");
	}

	// Update is called once per frame
	protected void Update () {
		if (isBuildingInProgress) {
			Building();
		} else if (isBuilt) {
			// actions
		}
	}

	protected void SetOriginalColour(Vector4 oc) {
		originalColour = oc;
	}

	protected void SetTopRightPoint(Point<int> topRight) {
		topRightPoint = Point<float>.ToIsometric(topRight);
	}

	protected void StartBuilding(Transform builder) {
		Builder builderScript = builder.GetComponent<Builder> ();
		combinedBuildingSpeed += builderScript.actionSpeed;
		player = builderScript.player;
		assignedUnits.Add (builder);
		isBuildingInProgress = true;
		if (healthPoints == 0) {
			int xSize = (int)System.Math.Ceiling (left.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth);
			int ySize = (int)System.Math.Ceiling (left.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) + 1;
			Point<int> wordPt = Point<int>.FromIsometric (new Point<float> (transform.position.x, transform.position.y));
			wordPt.x -= xSize;
			wordPt.y -= ySize;
			Debug.Log (xSize + " " + ySize);
			entranceExit = Vector3.zero;
			Point<float> tempPt = Point<float>.ToIsometric (wordPt);
			entranceExit = new Vector3(tempPt.x, tempPt.y, 0.0f);
			Debug.Log (entranceExit);
			flagPoint = wordPt;

			string path = "Prefabs/Flags/Flag";
			Transform flagPrefab = Resources.Load <Transform> (path);
			flagObject = Instantiate (flagPrefab, entranceExit, Quaternion.identity) as Transform;
			flagObject.GetComponent<Renderer> ().sortingOrder = 1;
			flagObject.parent = transform;

			flagObject.gameObject.SetActive (false);
		}
	}

	protected void Building() {
		amountBuilt += combinedBuildingSpeed * Time.deltaTime * 5000;
		int amount = (int)System.Math.Floor (amountBuilt);
		if (amount > 0) {
			amountBuilt -= amount;
			healthPoints += amount;
		}
		if (healthPoints >= maxHealthPoints) {
			healthPoints = maxHealthPoints;
			isBuilt = true;
			isBuildingInProgress = false;
			foreach (Transform builder in assignedUnits) {
				builder.SendMessage ("BuildingIsCompleted");
			}
			assignedUnits.Clear ();
			left.GetComponent<SpriteRenderer> ().color = originalColour;
			right.GetComponent<SpriteRenderer> ().color = originalColour;
		}
	}

	protected void ShowMenu() {
		GameObject canvas = GameObject.Find ("Canvas");
		if (isBuilt) {
			flagObject.gameObject.SetActive (true);
			canvas.SendMessage ("Selected", transform);
		} else {
			// TODO: show build progress
		}
	}

	protected void CreateCharacter(string characterType) {
		string path = "Prefabs/Characters/" + characterType;
		Transform unit = Resources.Load <Transform> (path);
		Transform newUnit = Instantiate (unit, entranceExit, Quaternion.identity) as Transform;
		newUnit.GetComponent<Renderer> ().sortingOrder = 2;
		newUnit.parent = characters.transform;
		char[] delimiter = { '(' };
		newUnit.name = newUnit.name.Split (delimiter) [0];
		Builder unitScript = newUnit.GetComponent<Builder> ();
		unitScript.player = player;
		newUnit.SendMessage ("AcquireTarget", flagPoint);
	}

	protected void PlaceFlag(Point<int> mapPt) {
		flagPoint.x = mapPt.x;
		flagPoint.y = mapPt.y;

		Point<float> isoPt = Point<float>.ToIsometric (mapPt);
		Vector3 flagPosition = flagObject.position;
		flagPosition.x = isoPt.x;
		flagPosition.y = isoPt.y;
		flagObject.position = flagPosition;
		flagObject.GetComponent<SpriteRenderer>().sortingOrder = Map.mapData[mapPt.x, mapPt.y].tile.GetComponent<Renderer>().sortingOrder + 1;
	}

	protected void UnSelect() {
		flagObject.gameObject.SetActive (false);
	}

	public virtual void UnitReturned (Character unit, string resource, int amount) {}
}
