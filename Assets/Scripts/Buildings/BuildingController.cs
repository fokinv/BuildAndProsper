using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController {
	private Player player;

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

	public BuildingController(Player pl) {
		player = pl;
	}

	public void PlaceBuilding (StructureData structure, List<Transform> selectedBuilders, Point<int> topRightPoint) {
		int xSize = (int) System.Math.Ceiling(structure.tile.Find("left").GetComponent<SpriteRenderer> ().bounds.size.x * 2 / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(structure.tile.Find("left").GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> bottomRightPoint = new Point<int> (topRightPoint.x, topRightPoint.y - ySize);
		Point<int> topLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y);
		Point<int> middlePoint = new Point<int> (topRightPoint.x - xSize / 2, topRightPoint.y - ySize / 2);

		for (int y = topRightPoint.y; y >= bottomRightPoint.y; y--) {
			for (int x = topRightPoint.x; x >= topLeftPoint.x; x--) {
				Map.mapData [x, y].isWalkable = false;
				Map.buildingData [x, y] = structure;
			}
		}
		int leftLayer = (Map.mapSizeX + Map.mapSizeY) - (topLeftPoint.x + topLeftPoint.y);
		int rightLayer = (Map.mapSizeX + Map.mapSizeY) - (bottomRightPoint.x + bottomRightPoint.y);


		structure.tile.parent = buildings.transform;
		SpriteRenderer leftSpriteRenderer = structure.tile.Find("left").GetComponent<SpriteRenderer> ();
		SpriteRenderer rightSpriteRenderer = structure.tile.Find("right").GetComponent<SpriteRenderer> ();
		leftSpriteRenderer.color = originalColour;
		leftSpriteRenderer.sortingOrder = leftLayer;
		rightSpriteRenderer.sortingOrder = rightLayer;
		rightSpriteRenderer.color = originalColour;
		structure.tile.tag = "Structures";
		foreach (Transform go in selectedBuilders) {
			go.SendMessage ("AcquireTarget", middlePoint);
		}
		structure.tile.SendMessage ("SetOriginalColour", originalColour);
		structure.tile.SendMessage ("SetTopRightPoint", topRightPoint);

		player.wood -= structure.script.woodCost;
		player.stone -= structure.script.stoneCost;
		originalColour = Vector4.zero;
	}

	public void CheckIfBuildable(StructureData structure, Point<int> topRightPoint) {
		isCharacterInWay = false;
		int xSize = (int) System.Math.Ceiling(structure.tile.Find("left").GetComponent<SpriteRenderer> ().bounds.size.x * 2 / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(structure.tile.Find("left").GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);

		if (!Point<int>.PointIsInMap(topRightPoint) || !Point<int>.PointIsInMap(bottomLeftPoint)) {
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
			Point<int> characterPosition = Point<int>.FromIsometric (new Point<float> (character.position.x, character.position.y));

			if (characterPosition.x >= bottomLeftPoint.x && characterPosition.x <= topRightPoint.x &&
				characterPosition.y >= bottomLeftPoint.y && characterPosition.y <= topRightPoint.y) {
				isCharacterInWay = true;
				break;
			}
		}

		bool hasEnoughResource = player.wood >= structure.script.woodCost && player.stone >= structure.script.stoneCost;

		if (acceptedSurface == surface && hasEnoughResource && !isCharacterInWay) {
			isCorrectlyPlaced = true;
		} else {
			isCorrectlyPlaced = false;
		}
	}

	public void ColorBuilding(StructureData structure) {
		if (originalColour == Vector4.zero) {
			originalColour= structure.tile.Find("left").GetComponent<SpriteRenderer> ().color;
		}
		Vector4 currentColor = structure.tile.Find("left").GetComponent<SpriteRenderer> ().color;
		if (isCorrectlyPlaced) {
			currentColor.x = 0;
			currentColor.y = 1;
			currentColor.z = 0;
		} else {
			currentColor.x = 1;
			currentColor.y = 0;
			currentColor.z = 0;
		}
		structure.tile.Find("left").GetComponent<SpriteRenderer> ().color = currentColor;
		structure.tile.Find("right").GetComponent<SpriteRenderer> ().color = currentColor;
	}
	//TODO: resources
}
