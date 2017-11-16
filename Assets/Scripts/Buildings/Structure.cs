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
	private float buildingSpeed = 100.0f;

	private Point<float> topRightPoint;
	private Point<float> bottomLeftPoint;
	private Point<int> flagPoint;
	private Vector3 entranceExit;

	private GameObject characters;
	private Transform flagObject;

	private List<Transform> builders = new List<Transform> ();
	//resources cost

	// Use this for initialization
	protected void Start () {
		characters = GameObject.Find ("CharactersObject");
	}

	// Update is called once per frame
	protected void Update () {
		if (isBuildingInProgress) {
			building();
		} else if (isBuilt) {
			// actions
		}
	}

	protected void setOriginalColour(Vector4 oc) {
		originalColour = oc;
	}

	protected void setTopRightPoint(Point<int> topRight) {
		topRightPoint = Point<float>.toIsometric(topRight);
	}

	protected void setBottomLeftPoint(Point<int> bottomLeft) {
		bottomLeftPoint = Point<float>.toIsometric(bottomLeft);
	}

	protected void startBuilding(Transform builder) {
		Debug.Log ("StartBuilding");
		builders.Add (builder);
		isBuildingInProgress = true;
		if (healthPoints == 0) {
			int xSize = (int)System.Math.Ceiling (GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) / 2;
			int ySize = (int)System.Math.Ceiling (GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) + 1;
			Point<int> wordPt = Point<int>.fromIsometric (new Point<float> (transform.position.x, transform.position.y));
			wordPt.x -= xSize;
			wordPt.y -= ySize;
			entranceExit = Vector3.zero;
			entranceExit.x = Point<float>.toIsometric (wordPt).x;
			entranceExit.y = Point<float>.toIsometric (wordPt).y;
			flagPoint = wordPt;

			string path = "Prefabs/Flags/Flag";
			Transform flagPrefab = Resources.Load <Transform> (path);
			flagObject = Instantiate (flagPrefab, entranceExit, Quaternion.identity) as Transform;
			flagObject.GetComponent<Renderer> ().sortingOrder = 2;
			flagObject.parent = transform;

			flagObject.gameObject.SetActive (false);
		}
	}

	protected void building() {
		amountBuilt += buildingSpeed * Time.deltaTime * 50;
		int amount = (int)System.Math.Floor (amountBuilt);
		if (amount > 0) {
			amountBuilt -= amount;
			healthPoints += amount;
		}
		if (healthPoints >= maxHealthPoints) {
			healthPoints = maxHealthPoints;
			isBuilt = true;
			isBuildingInProgress = false;
			foreach (Transform builder in builders) {
				builder.SendMessage ("buildingIsCompleted");
			}
			builders.Clear ();
			GetComponent<SpriteRenderer> ().color = originalColour;
		}
	}

	protected void showMenu() {
		flagObject.gameObject.SetActive (true);
		GameObject canvas = GameObject.Find ("Canvas");
		if (isBuilt) {
			canvas.SendMessage ("selected", transform);
		} else {
			// TODO: show build progress
		}
	}

	protected void createCharacter(string characterType) {
		string path = "Prefabs/Characters/" + characterType;
		Transform character = Resources.Load <Transform> (path);
		Transform newCharacter = Instantiate (character, entranceExit, Quaternion.identity) as Transform;
		newCharacter.GetComponent<Renderer> ().sortingOrder = 2;
		newCharacter.parent = characters.transform;
		char[] delimiter = { '(' };
		newCharacter.name = newCharacter.name.Split (delimiter) [0];
		newCharacter.SendMessage ("acquireTarget", flagPoint);
	}

	protected void placeFlag(Point<int> mapPt) {
		Debug.Log (mapPt.x + " " + mapPt.y);
		flagPoint.x = mapPt.x;
		flagPoint.y = mapPt.y;

		Point<float> isoPt = Point<float>.toIsometric (mapPt);
		Vector3 flagPosition = flagObject.position;
		flagPosition.x = isoPt.x;
		flagPosition.y = isoPt.y;
		flagObject.position = flagPosition;
	}

	protected void unSelect() {
		flagObject.gameObject.SetActive (false);
		Debug.Log (flagObject.gameObject.activeSelf);
		Debug.Log (flagObject.gameObject.activeInHierarchy);
	}

	void OnGUI() {
		if (isBuildingInProgress) {
			Texture2D greyBox = new Texture2D (1,1);
			greyBox.SetPixel (0, 0, new Color(0.5f, 1.0f, 0.5f, 1.0f));
			greyBox.Apply();
			Rect rect = new Rect (topRightPoint.x, Screen.height - topRightPoint.y, bottomLeftPoint.x - topRightPoint.x, -1 * (bottomLeftPoint.y - topRightPoint.y));
			GUI.DrawTexture (rect, greyBox);
		}
	}
}
