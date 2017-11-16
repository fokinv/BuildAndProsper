using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment {

	public static void createEnvironment(Point<int> middle, int radius, TileData.TileType type) {
		bool isWalkable;
		if (type == TileData.TileType.Hill) {
			isWalkable = true;
		} else {
			isWalkable = false;
		}
		Point<int> buttomLeft = new Point<int> (middle.x - radius, middle.y - radius);
		Point<int> topRight = new Point<int> (middle.x + radius, middle.y + radius);


		if (type == TileData.TileType.Hill || type == TileData.TileType.Lake) {
			if (radius == 0) {
				Map.mapData [middle.x, middle.y] = new TileData (1, isWalkable, type);
			} else {
				for (int y = Map.mapSizeY - 1; y >= 0; y--) {
					for (int x = Map.mapSizeX - 1; x >= 0; x--) {
						if (y < buttomLeft.y || y > topRight.y || x < buttomLeft.x || x > topRight.x) {
							continue;
						} else if (y == buttomLeft.y) {
							if (x == buttomLeft.x) {
								//Debug.Log ("A2");
								Map.mapData [x, y].changeTile (2, isWalkable, type);
							} else if (x == topRight.x) {
								//Debug.Log ("A3");
								Map.mapData [x, y].changeTile (5, isWalkable, type);
							} else if (x < topRight.x && x > buttomLeft.x) {
								//Debug.Log ("A4");
								Map.mapData [x, y].changeTile (9, isWalkable, type);
							}
						} else if (y == topRight.y) {
							if (x == topRight.x) {
								//Debug.Log ("A5");
								Map.mapData [x, y].changeTile (4, isWalkable, type);
							} else if (x == buttomLeft.x) {
								//Debug.Log ("A5");
								Map.mapData [x, y].changeTile (3, isWalkable, type);
							} else if (x < topRight.x && x > buttomLeft.x) {
								//Debug.Log ("A7");
								Map.mapData [x, y].changeTile (7, isWalkable, type);
							}
						} else if (x == buttomLeft.x) {
							if (y < topRight.y && y > buttomLeft.y) {
								Map.mapData [x, y].changeTile (6, isWalkable, type);
							}
						} else if (x == topRight.x) {
							if (y < topRight.y && y > buttomLeft.y) {
								Map.mapData [x, y].changeTile (8, isWalkable, type);
							}
						} else {
							Map.mapData [x, y].changeTile (1, isWalkable, type);
						}
					}
				}
			} 
		} else if (type == TileData.TileType.Forest || type == TileData.TileType.Stone) {
			for (int y = topRight.y; y >= buttomLeft.y; y--) {
				for (int x = topRight.x; x >= buttomLeft.x; x--) {
					if (Map.mapData [x, y].tileType == TileData.TileType.Ground) {
						Map.mapData [x, y].isWalkable = false;
						Map.resourceData [x, y] = new TileData (1, false, TileData.TileType.Forest);
					}
				}
			}
		}
	}
}
