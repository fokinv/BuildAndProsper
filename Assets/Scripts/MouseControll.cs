using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControll : MonoBehaviour {
	public static Vector3 mousePosition { get; set; }
	private Vector3 world;
	private int bound = 50;
	private int screenHeight;
	private int screenWidth;
	private int speed = 3;
	private static Vector3 transPos;
	//private Vector3 target;
	private GameObject character;
	List<Point<int>> path;
	private bool isCorrectlyPlaced = false;
	private static Transform tileObject = null;
	private Vector4 originalColour = new Vector4();

	public static bool isBuilding { get; set; }

	// Use this for initialization
	void Start () {
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		//Debug.Log (screenWidth + "*" + screenHeight);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (map.x + " " + map.y);
		mousePosition = Input.mousePosition;
		//Debug.Log (mousePosition.x + " " + mousePosition.y);
		moveCameraIfMouseAtEdge();
		processInputEvent ();
		if (isBuilding) {
			moveBuilding ();
			checkIfBuildable ();
			colorBuilding ();
		}
	}
		
	private void moveCameraIfMouseAtEdge() {
		transPos = transform.position;
		float mapWidth = Map.mapSizeX * InitMap.tileWidth;
		float mapHeight = Map.mapSizeY * InitMap.tileHeight;
		if (mousePosition.x > screenWidth - bound) {
			//transPos.x += speed * Time.deltaTime;
			moveCamera (1, 0);
		}
		if (mousePosition.y > screenHeight - bound) {
			//transPos.y += speed * Time.deltaTime;
			moveCamera (0, 1);
		}
		if (mousePosition.x < 0 + bound) {
			//transPos.x -= speed * Time.deltaTime;
			moveCamera (-1, 0);
		}
		if (mousePosition.y < 0 + bound) {
			//transPos.y -= speed * Time.deltaTime;
			moveCamera (0, -1);
		}

	}

	private void processInputEvent () {
		if (Input.GetMouseButtonDown(0)) {
			/*Vector3 mousePosition = Input.mousePosition;
			Vector3 iso_pt = Camera.main.ScreenToWorldPoint (mousePosition);
			Point<int> target = Point<int>.fromScreen(mousePosition);
			Point<int> target2 = Point<int>.fromIsometric(new Point<float>(iso_pt.x, iso_pt.y));
			Destroy (Map.mapData[target.x, target.y].tile.gameObject);*/
			if (isBuilding) {
				// build if possible
				if (isCorrectlyPlaced) {
					placeBuilding ();
				} else {
					// TODO: show message
				}
			} else {
				// select
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			// unselect
		}
		if (Input.GetKey(KeyCode.W)) {
			moveCamera (0, 1);
		}
		if (Input.GetKey(KeyCode.A)) {
			moveCamera (-1, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			moveCamera (0, -1);
		}
		if (Input.GetKey(KeyCode.D)) {
			moveCamera (1, 0);
		}
		if (Input.GetKey(KeyCode.Escape)) {
			cancelBuilding();
		}

	}

	private void moveCamera(int signX, int signY) {
		transPos.x += signX * speed * Time.deltaTime;
		transPos.y += signY * speed * Time.deltaTime;
		transform.position = transPos;
	}

	public static void loadBuilding(string path) {
		MouseControll.isBuilding = true;

		if (tileObject == null) {
			Point<int> isoPt = Point<int>.fromScreen (Input.mousePosition);
			Transform building = Resources.Load <Transform> (path);
			tileObject = Instantiate (building, new Vector3 (isoPt.x, isoPt.y, 0), Quaternion.identity) as Transform;
			tileObject.GetComponent<Renderer> ().sortingOrder = 1;
		}

		isBuilding = true;
	}

	private void moveBuilding() {
		Vector3 buildingPos = tileObject.position;
		Point<int> isoPt = Point<int>.fromScreen (Input.mousePosition);
		Point<float> screenPt = Point<float>.toIsometric (isoPt);
		Debug.Log ("screenPt X: " + screenPt.x + " Y: " + screenPt.y);
		Debug.Log ("position X: " + tileObject.position.x + " Y: " + tileObject.position.y);
		buildingPos.x = screenPt.x;
		buildingPos.y = screenPt.y;
		tileObject.position = buildingPos;
	}

	private void checkIfBuildable() {
		int xSize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> topRightPoint = Point<int>.fromScreen (Input.mousePosition);
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);
		//Point<float> screenPt = Point<float>.toIsometric (middlePoint);

		//Temporary workaround, can implement when with more building
		//Point<int> bottomLeftPoint = new Point<int> (middlePoint.x - xSize, middlePoint.y - ySize);
		//Debug.Log ("topRightPoint X: " + topRightPoint.x + " Y: " + topRightPoint.y);
		//Debug.Log ("bottomLeftPoint X: " + bottomLeftPoint.x + " Y: " + bottomLeftPoint.y);
		if (/*!Point<int>.pointIsInMap(middlePoint) ||*/ !Point<int>.pointIsInMap(topRightPoint) || !Point<int>.pointIsInMap(bottomLeftPoint)) {
			isCorrectlyPlaced = false;
			return;
		}

		int surface = 0;
		int acceptedSurface = 0;
		for (int y = topRightPoint.y; y >= bottomLeftPoint.y; y--) {
			for (int x = topRightPoint.x; x >= bottomLeftPoint.x; x--) {
				surface++;
				if (Map.mapData [x, y].tileType == TileData.TileType.Ground && Map.mapData [x, y].isWalkable) {
					acceptedSurface++;
				}
			}
		}
		if (acceptedSurface == surface) {
			isCorrectlyPlaced = true;
		} else {
			isCorrectlyPlaced = false;
		}

	}

	private void colorBuilding() {
		if (originalColour == new Vector4()) {
			originalColour= tileObject.GetComponent<SpriteRenderer> ().color;
		}
		Vector4 currentColor = tileObject.GetComponent<SpriteRenderer> ().color;
		if (isCorrectlyPlaced) {
			//tileObject.GetComponent<SpriteRenderer> ().color.r = 1;
			currentColor.x = 0;
			currentColor.y = 1;
			currentColor.z = 0;
		} else {
			currentColor.x = 1;
			currentColor.y = 0;
			currentColor.z = 0;
			//tileObject.GetComponent<SpriteRenderer> ().color.g = 1;
		}
		tileObject.GetComponent<SpriteRenderer> ().color = currentColor;
	}

	private void placeBuilding() {
		int xSize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> topRightPoint = Point<int>.fromScreen (Input.mousePosition);
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);

		for (int y = topRightPoint.y; y >= bottomLeftPoint.y; y--) {
			for (int x = topRightPoint.x; x >= bottomLeftPoint.x; x--) {
				Map.mapData [x, y].isWalkable = false;
				Map.buildingData [x, y] = new TileData (0, false, TileData.TileType.Building, tileObject);
			}
		}
		isBuilding = false;
		tileObject.GetComponent<SpriteRenderer> ().color = originalColour;
		originalColour = new Vector4 ();
		tileObject = null;
	}
	
	private void cancelBuilding() {
		isBuilding = false;
		if (tileObject != null) {
			Destroy (tileObject.gameObject);
		}
		tileObject = null;
	}



	public static Vector3 getMouseWoldPosition() {
		mousePosition = Input.mousePosition;
		return Camera.main.ScreenToWorldPoint (mousePosition);
	}
}
