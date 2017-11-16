using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController {
	public enum ActionType
	{
		Error,
		PlaceFlag,
	}

	private GameObject buildings = GameObject.Find ("StructuresObject");
	private GameObject characters = GameObject.Find ("CharactersObject");
	private Vector4 originalColour = new Vector4();
	public bool isCorrectlyPlaced { get; set; }
	public bool isCharacterInWay { get; set; }

	public void placeBuilding (Transform structure, List<Transform> selectedBuilders, Point<int> topRightPoint) {
		int xSize = (int) System.Math.Ceiling(structure.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(structure.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);
		Point<int> middlePoint = new Point<int> (topRightPoint.x - xSize / 2, topRightPoint.y - ySize / 2);

		for (int y = topRightPoint.y; y >= bottomLeftPoint.y; y--) {
			for (int x = topRightPoint.x; x >= bottomLeftPoint.x; x--) {
				Map.mapData [x, y].isWalkable = false;
				Map.buildingData [x, y] = new TileData (0, false, TileData.TileType.Building, structure);
			}
		}
		structure.parent = buildings.transform;
		structure.GetComponent<SpriteRenderer> ().color = originalColour;
		structure.tag = "Structures";
		foreach (Transform go in selectedBuilders) {
			go.SendMessage ("acquireTarget", middlePoint);
		}
		structure.SendMessage ("setOriginalColour", originalColour);
		structure.SendMessage ("setTopRightPoint", topRightPoint);
		structure.SendMessage ("setBottomLeftPoint", bottomLeftPoint);

		originalColour = new Vector4 ();
	}

	public void checkIfBuildable(Transform structure, Point<int> topRightPoint) {
		isCharacterInWay = false;
		int xSize = (int) System.Math.Ceiling(structure.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(structure.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
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

		foreach (Transform character in characters.GetComponentInChildren<Transform>()) {
			bool isXWithin = false;
			bool isYWithin = false;
			Point<int> characterPosition = Point<int>.fromIsometric (new Point<float> (character.position.x, character.position.y));

			if (characterPosition.x >= bottomLeftPoint.x && characterPosition.x <= topRightPoint.x &&
				characterPosition.y >= bottomLeftPoint.y && characterPosition.y <= topRightPoint.y) {
				isCharacterInWay = true;
				break;
			}
		}

		if (acceptedSurface == surface && !isCharacterInWay) {
			isCorrectlyPlaced = true;
		} else {
			isCorrectlyPlaced = false;
		}
	}

	public void colorBuilding(Transform structure) {
		if (originalColour == Vector4.zero) {
			originalColour= structure.GetComponent<SpriteRenderer> ().color;
		}
		Vector4 currentColor = structure.GetComponent<SpriteRenderer> ().color;
		if (isCorrectlyPlaced) {
			currentColor.x = 0;
			currentColor.y = 1;
			currentColor.z = 0;
		} else {
			currentColor.x = 1;
			currentColor.y = 0;
			currentColor.z = 0;
		}
		structure.GetComponent<SpriteRenderer> ().color = currentColor;
	}
	//TODO: resources
}
