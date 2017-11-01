using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment {

	public Point<int> middle { get; set; }
	public int radius { get; set; }
	private TileData.TileType type;

	public Environment(Point<int> middle, int radius, TileData.TileType type) {
		this.middle = middle;
		this.radius = radius;
		this.type = type;

		bool isWalkable;
		if (type == TileData.TileType.Hill) {
			isWalkable = true;
		} else {
			isWalkable = false;
		}

		//Debug.Log (radius);
		if (radius == 0) {
			Map.mapData [middle.x, middle.y] = new TileData(1, isWalkable, type);
		} else 
		{
			Point<int> buttomLeft = new Point<int> (middle.x - radius, middle.y - radius);
			Point<int> topRight = new Point<int> (middle.x + radius, middle.y + radius);
			for (int y = Map.mapSizeY - 1; y >= 0; y--) {
				for (int x = Map.mapSizeX - 1; x >= 0; x--) {
					if (y < buttomLeft.y || y > topRight.y || x < buttomLeft.x || x > topRight.x) {
						//Debug.Log ("A1");
						continue;
					} /*else if (getNeighbourEnvironmentAmount (x, y) >= 7) {
						InitMap.mapData [x, y].changeTile(1, isWalkable, type);
					}*/ else if (y == buttomLeft.y) {
						if (x == buttomLeft.x) {
							//Debug.Log ("A2");
							Map.mapData [x, y].changeTile(2, isWalkable, type);
						} else if (x == topRight.x) {
							//Debug.Log ("A3");
							Map.mapData [x, y].changeTile(5, isWalkable, type);
						} else if (x < topRight.x && x > buttomLeft.x) {
							//Debug.Log ("A4");
							Map.mapData [x, y].changeTile(9, isWalkable, type);
						}
					} else if (y == topRight.y) {
						if (x == topRight.x) {
							//Debug.Log ("A5");
							Map.mapData [x, y].changeTile(4, isWalkable, type);
						} else if (x == buttomLeft.x) {
							//Debug.Log ("A5");
							Map.mapData [x, y].changeTile(3, isWalkable, type);
						} else if (x < topRight.x && x > buttomLeft.x) {
							//Debug.Log ("A7");
							Map.mapData [x, y].changeTile(7, isWalkable, type);
						}
					} else if (x == buttomLeft.x) {
						if (y < topRight.y && y > buttomLeft.y) {
							Map.mapData [x, y].changeTile(6, isWalkable, type);
						}
					} else if (x == topRight.x) {
						if (y < topRight.y && y > buttomLeft.y) {
							Map.mapData [x, y].changeTile(8, isWalkable, type);
						}
					} else {
						Map.mapData [x, y].changeTile(1, isWalkable, type);
					}
				}
			}
		}
	}
}
