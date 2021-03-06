﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseControll : Player {
	public static Vector3 mousePosition { get; set; }
	private Vector3 world;
	private int bound = 50;
	private int screenHeight;
	private int screenWidth;
	private int speed = 3;
	private static Vector3 transPos;
	//private Vector3 target;
	List<Point<int>> path;
	private static StructureData structureToBuild = null;
	private Vector4 originalColour = Vector4.zero;

	private List<Transform> selectedGameObjects = new List<Transform>();


	private Vector3 originBoxPos = Vector3.zero;
	private Vector3 endBoxPos = Vector3.zero;


	Point<int> topRightPoint;

	private bool isOverGUI = false;

	GameObject canvas;

	public static bool isBuilding { get; set; }
	private bool isPlacingFlag = false;

	public static List<string> availableCharacters { get; set; }
	public static List<string> availableStructures { get; set; }

	void Start () {
		availableCharacters = new List<string>();
		availableStructures = new List<string>();
		availableCharacters.Add ("Builder");
		availableStructures.Add ("Castle");
		availableStructures.Add ("Chapel");

		structures = GameObject.Find ("StructuresObject");
		characters = GameObject.Find ("CharactersObject");
		canvas = GameObject.Find ("Canvas");
		wood = 100;
		stone = 100;
		food = 100;

		buildingController = new BuildingController(this);
		if (structures == null) {
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
		MoveCameraIfMouseAtEdge();
		ProcessInputEvent ();
		if (isBuilding) {
			topRightPoint = Point<int>.FromScreen (Input.mousePosition);
			MoveBuilding ();
			buildingController.CheckIfBuildable (structureToBuild, topRightPoint);
			buildingController.ColorBuilding (structureToBuild);
		}
	}

	private void MoveCameraIfMouseAtEdge() {
		transPos = transform.position;

		Vector3 top = Map.mapData [Map.mapSizeX - 1, Map.mapSizeY - 1].tile.transform.position;
		Vector3 left = Map.mapData [0, Map.mapSizeY - 1].tile.transform.position;
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
			MoveCamera (1, 0);
		}
		if (mousePosition.y > screenHeight - bound && worldTop.y > screenHeight) {
			//transPos.y += speed * Time.deltaTime;
			MoveCamera (0, 1);
		}
		if (mousePosition.x < 0 + bound && worldLeft.x < 0) {
			//transPos.x -= speed * Time.deltaTime;
			MoveCamera (-1, 0);
		}
		if (mousePosition.y < 0 + bound && worldBottom.y < 0) {
			//transPos.y -= speed * Time.deltaTime;
			MoveCamera (0, -1);
		}
	}

	private void AddSelectedGameObject(Transform selected) {
		selectedGameObjects.Add(selected);
	}

	// Check if structure is clicked
	private void StructureClicked(Vector3 mousePosition) {
		Point<int> isoPt = Point<int>.FromScreen (mousePosition);
		if (isoPt.x < Map.mapSizeX && isoPt.x >= 0 && isoPt.y < Map.mapSizeY && isoPt.y >= 0) {
			if (Map.buildingData [isoPt.x, isoPt.y] != null) {
				// TODO: check if player
				if (Map.buildingData [isoPt.x, isoPt.y] != null) {
					selectedGameObjects.Add (Map.buildingData [isoPt.x, isoPt.y].tile);
					selectedGameObjects[0].SendMessage ("ShowMenu");
				} 
			} else {
				foreach (Transform go in selectedGameObjects) {
					go.SendMessage ("UnSelect");
				}
				canvas.SendMessage ("NothingSelected");
				selectedGameObjects.Clear ();
			}
		}
	}

	// Check if character is clicked
	private void CharacterClicked(Vector3 mousePosition) {
		Vector2 origin = new Vector2 (Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);
		RaycastHit2D hit = Physics2D.Raycast (origin, Vector2.zero, 0f);
		//Vector4 asd = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
		//hit.transform.GetComponent<SpriteRenderer> ().color = asd;
		if (hit) {
			// TODO: check if player
			hit.transform.SendMessage ("ShowMenu");
			selectedGameObjects.Add (hit.transform);
		} else {
			StructureClicked (mousePosition);
		}
	}

	private void ProcessInputEvent () {
		if (Input.GetMouseButtonDown(0)) {
			Point<int> mapPt = Point<int>.FromScreen (mousePosition);
			if (!isOverGUI) {
				if (mapPt.x < Map.mapSizeX && mapPt.y < Map.mapSizeY) {
					if (isBuilding) {
						// Build if possible
						if (buildingController.isCorrectlyPlaced) {
							buildingController.PlaceBuilding (structureToBuild, selectedGameObjects, topRightPoint);
							/*foreach (Transform go in selectedGameObjects) {
								go.SendMessage ("acquireTarget", topRightPoint);
							}*/
							isBuilding = false;
							structureToBuild = null;
						} else {
							// TODO: show message
						}
					} else if(isPlacingFlag) {
						selectedGameObjects [0].SendMessage ("PlaceFlag", mapPt);
						isPlacingFlag = false;
					} else {
						if (selectedGameObjects.Count != 0) {
							foreach (Transform go in selectedGameObjects) {
								go.SendMessage ("UnSelect");
							}
							canvas.SendMessage ("NothingSelected");
							selectedGameObjects.Clear ();
						}
						// Single selection
						Vector3 mousePosition = Input.mousePosition;
						CharacterClicked (mousePosition);
					}
				}
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			if (!isBuilding) {
				if (selectedGameObjects.Count != 0) {
					foreach (Transform go in selectedGameObjects) {
						if (availableCharacters.Contains (go.name)) {
							go.SendMessage ("AcquireTarget", Point<int>.FromScreen (mousePosition));
						}
					}
				}
			}
		}
		// Box for multiple selection
		if (Input.GetMouseButton (0)) {
			if (Input.GetMouseButtonDown (0)) {
				originBoxPos = Input.mousePosition;
			} else {
				endBoxPos = Input.mousePosition;
			}
		} else {
			if (originBoxPos != Vector3.zero && endBoxPos != Vector3.zero) {
				//selectedGameObjects.Clear ();
				Vector3 origin = Camera.main.ScreenToWorldPoint(originBoxPos);
				Vector3 end = Camera.main.ScreenToWorldPoint(endBoxPos);
				Vector4 parameters = new Vector4(origin.x, origin.y, end.x, end.y);
				characters.BroadcastMessage("CheckIfInSelecionRectangle", parameters);
				if (selectedGameObjects.Count > 0) {
					bool isSameType = true;
					string firstType = selectedGameObjects [0].name;
					foreach (Transform character in selectedGameObjects) {
						if (character.name != firstType) {
							isSameType = false;
							break;
						}
					}
					if (isSameType) {
						selectedGameObjects [0].SendMessage ("ShowMenu");
					} else {
						//TODO: show default menu
					}
				}
			}
			originBoxPos = endBoxPos = Vector3.zero;
		}
		if (Input.GetKey(KeyCode.W)) {
			MoveCamera (0, 1);
		}
		if (Input.GetKey(KeyCode.A)) {
			MoveCamera (-1, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			MoveCamera (0, -1);
		}
		if (Input.GetKey(KeyCode.D)) {
			MoveCamera (1, 0);
		}
		if (Input.GetKey(KeyCode.Escape)) {
			if (isBuilding) {
				CancelBuilding ();
			}
			canvas.SendMessage ("NothingSelected");
			selectedGameObjects.Clear ();
		}
	}

	// Selection box drawing
	void OnGUI() {
		if (originBoxPos != Vector3.zero && endBoxPos != Vector3.zero) {
			Texture2D selectionTexture = new Texture2D (1,1);
			selectionTexture.SetPixel (0, 0, /*0Color.green*/ new Color(0f, 0.8f, 0f, 0.25f));
			selectionTexture.Apply();
			Rect rect = new Rect (originBoxPos.x, Screen.height - originBoxPos.y, endBoxPos.x - originBoxPos.x, -1 * (endBoxPos.y - originBoxPos.y));
			GUI.DrawTexture (rect, selectionTexture);
		}
	}

	private void MoveCamera(int signX, int signY) {
		transPos.x += signX * speed * Time.deltaTime;
		transPos.y += signY * speed * Time.deltaTime;
		transform.position = transPos;
	}

	private void HandleButtonClick(string prefabName) {
		string path = "";
		Vector3 isometricVec = Vector3.zero;
		if (availableStructures.Contains (prefabName)) {
			if (structureToBuild == null) {
				isometricVec = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				isometricVec.z = 0;
				path = "Prefabs/Buildings/" + prefabName + "/" + prefabName;
				Transform structure = Resources.Load <Transform> (path);
				Transform structureTile = Instantiate (structure, isometricVec, Quaternion.identity) as Transform;
				structureTile.Find("left").GetComponent<Renderer> ().sortingOrder = Map.mapData[0, 0].tile.GetComponent<Renderer> ().sortingOrder + 1;
				structureTile.Find("right").GetComponent<Renderer> ().sortingOrder = Map.mapData[0, 0].tile.GetComponent<Renderer> ().sortingOrder + 1;
				char[] delimiter = { '(' };
				structureTile.name = structureTile.name.Split (delimiter) [0];

				structureToBuild = new StructureData (StructureData.StructureType.Castle, structureTile);
			}
			isBuilding = true;
		} else if (availableCharacters.Contains (prefabName)) {
			selectedGameObjects [0].SendMessage ("CreateCharacter", prefabName);
		} else {
			BuildingController.ActionType actionType = ConvertButtonTextToActionType (prefabName);

			switch (actionType) {
			case BuildingController.ActionType.PlaceFlag:
				//TODO: write message: Place rally flag
				isPlacingFlag = true;
				break;
			}
		}
	}

	private BuildingController.ActionType ConvertButtonTextToActionType(string prefabName) {
		switch (prefabName) {
		case "Flag":
				return BuildingController.ActionType.PlaceFlag;
		default:
			return BuildingController.ActionType.Error;
		}
	}

	private void MoveBuilding() {
		Vector3 buildingPos = structureToBuild.tile.position;
		Point<int> mapPt = Point<int>.FromScreen (Input.mousePosition);
		Point<float> isoPt = Point<float>.ToIsometric (mapPt);
		buildingPos.x = isoPt.x;
		buildingPos.y = isoPt.y;
		structureToBuild.tile.position = buildingPos;
	}

	private void CancelBuilding() {
		isBuilding = false;
		if (structureToBuild != null) {
			Destroy (structureToBuild.tile.gameObject);
		}
		structureToBuild = null;
	}

	private void SetIsOverGUI (bool value) {
		isOverGUI = value;
	}

	public static Vector3 GetMouseWoldPosition() {
		mousePosition = Input.mousePosition;
		return Camera.main.ScreenToWorldPoint (mousePosition);
	}
}
