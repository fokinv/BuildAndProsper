using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController {
	private GameObject buildings = GameObject.Find ("Buildings");
	Transform transform;
	private Vector4 originalColour = new Vector4();

	/*void placeBuilding () {
		int xSize = (int) System.Math.Ceiling(transform.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(transform.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
		Point<int> topRightPoint = Point<int>.fromScreen (Input.mousePosition);
		Point<int> bottomLeftPoint = new Point<int> (topRightPoint.x - xSize, topRightPoint.y - ySize);

		for (int y = topRightPoint.y; y >= bottomLeftPoint.y; y--) {
			for (int x = topRightPoint.x; x >= bottomLeftPoint.x; x--) {
				Map.mapData [x, y].isWalkable = false;
				Map.buildingData [x, y] = new TileData (0, false, TileData.TileType.Building, transform);
			}
		}
		isBuilding = false;
		transform.GetComponent<SpriteRenderer> ().color = originalColour;
		transform.parent = buildings.transform;
		originalColour = new Vector4 ();
		tileObject = null;
	}

	private void checkIfBuildable() {
		int xSize = (int) System.Math.Ceiling(transform.GetComponent<SpriteRenderer> ().bounds.size.x / InitMap.tileWidth) - 1;
		int ySize = (int) System.Math.Ceiling(transform.GetComponent<SpriteRenderer> ().bounds.size.y / InitMap.tileHeight) - 1;
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

	}*/
	//TODO: resources
}
