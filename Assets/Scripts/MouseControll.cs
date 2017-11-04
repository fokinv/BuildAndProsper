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
	List<Point<int>> path;
	private bool isCorrectlyPlaced = false;
	private static Transform tileObject = null;
	private Vector4 originalColour = Vector4.zero;
	private GameObject buildings;
	private GameObject characters;
	public static GameObject selectedGameObject = null;

	private Vector2 originBoxPos = Vector2.zero;
	private Vector2 endBoxPos = Vector2.zero;

	public static bool isBuilding { get; set; }

	void Start () {
		buildings = GameObject.Find ("Buildings");
		characters = GameObject.Find ("Characters");
		if (buildings == null) {
			// TODO: show message
		}
		if (characters == null) {
			// TODO: show message
		}
		screenHeight = Screen.height;
		screenWidth = Screen.width;
	}

	void Update () {
		mousePosition = Input.mousePosition;
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

		Vector3 top = Map.mapData [Map.mapSizeX - 1, Map.mapSizeY - 1].tile.transform.position;
		Vector3 left = Map.mapData [0, Map.mapSizeX - 1].tile.transform.position;
		Vector3 right = Map.mapData [Map.mapSizeX - 1, 0].tile.transform.position;
		Vector3 bottom = Map.mapData [0, 0].tile.transform.position;

		Vector3 worldTop = Camera.main.WorldToScreenPoint (top);
		Vector3 worldLeft = Camera.main.WorldToScreenPoint (left);
		Vector3 worldRight = Camera.main.WorldToScreenPoint (right);
		Vector3 worldBottom = Camera.main.WorldToScreenPoint (bottom);
		float mapWidth = Map.mapSizeX * InitMap.tileWidth;
		float mapHeight = Map.mapSizeY * InitMap.tileHeight;

		if (mousePosition.x > screenWidth - bound && worldRight.x > screenWidth) {
			//transPos.x += speed * Time.deltaTime;
			moveCamera (1, 0);
		}
		if (mousePosition.y > screenHeight - bound && worldTop.y > screenHeight) {
			//transPos.y += speed * Time.deltaTime;
			moveCamera (0, 1);
		}
		if (mousePosition.x < 0 + bound && worldLeft.x < 0) {
			//transPos.x -= speed * Time.deltaTime;
			moveCamera (-1, 0);
		}
		if (mousePosition.y < 0 + bound && worldBottom.y < 0) {
			//transPos.y -= speed * Time.deltaTime;
			moveCamera (0, -1);
		}
	}

	/*private void updateSelectedGameObject(GameObject selected) {
		selectedGameObject = selected;
		characters.BroadcastMessage ("checkIfSelected");
		//buildings.BroadcastMessage ("checkIfSelected");
	}*/

	private void processInputEvent () {
		if (Input.GetMouseButtonDown(0)) {
			if (isBuilding) {
				// build if possible
				if (isCorrectlyPlaced) {
					placeBuilding ();
				} else {
					// TODO: show message
				}
			} else {
				Vector2 origin = new Vector2 (Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
				Debug.Log (origin);
				RaycastHit2D hit = Physics2D.Raycast (origin, Vector2.zero, 0f);
				if (hit) {
					selectedGameObject = hit.transform.gameObject;
					selectedGameObject.SendMessage ("select");
				} else {
					selectedGameObject = null;
					characters.BroadcastMessage ("unSelect");
				}
				// select
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			// if selected unit move/action if appropriate
			//sendAppropriateMessage();
		}
		if (Input.GetMouseButton (0)) {
			if (Input.GetMouseButtonDown (0)) {
				originBoxPos = Input.mousePosition;
			} else {
				endBoxPos = Input.mousePosition;
			}
		} else {
			if (originBoxPos != Vector2.zero && endBoxPos != Vector2.zero) {
				//TODO: Handle selection
			}
			originBoxPos = endBoxPos = Vector2.zero;
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
			// TODO: cancel select
			selectedGameObject = null;
			characters.BroadcastMessage("unSelect") ;
		}
	}

	void OnGUI() {
		if (originBoxPos != Vector2.zero && endBoxPos != Vector2.zero) {
			Texture2D selectionTexture = new Texture2D (1,1);
			selectionTexture.SetPixel (0, 0, /*0Color.green*/ new Color(0f, 0.8f, 0f, 0.25f));
			selectionTexture.Apply();
			var rect = new Rect (originBoxPos.x, Screen.height - originBoxPos.y, endBoxPos.x - originBoxPos.x, -1 * (endBoxPos.y - originBoxPos.y));
			GUI.DrawTexture (rect, selectionTexture);
		}
	}

	/*private void sendAppropriateMessage() {
		characters.BroadcastMessage(" ;
	}*/

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
		buildingPos.x = screenPt.x;
		buildingPos.y = screenPt.y;
		tileObject.position = buildingPos;
	}

	private void checkIfBuildable() {
		int xSize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(tileObject.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> topRightPoint = Point<int>.fromScreen (Input.mousePosition);
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);

		if (!Point<int>.pointIsInMap(topRightPoint) || !Point<int>.pointIsInMap(bottomLeftPoint)) {
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
		if (originalColour == Vector4.zero) {
			originalColour= tileObject.GetComponent<SpriteRenderer> ().color;
		}
		Vector4 currentColor = tileObject.GetComponent<SpriteRenderer> ().color;
		if (isCorrectlyPlaced) {
			currentColor.x = 0;
			currentColor.y = 1;
			currentColor.z = 0;
		} else {
			currentColor.x = 1;
			currentColor.y = 0;
			currentColor.z = 0;
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
		tileObject.parent = buildings.transform;
		originalColour = Vector4.zero;
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
