using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point<T> {
	public T x { get; set; }
	public T y { get; set; }

	public Point(T x, T y) {
		this.x = x;
		this.y = y;
	}

	public Point() {
		this.x = default(T);
		this.y = default(T);
	}

	public static bool pointIsInMap(Point<int> pt) {
		if (pt.x >= Map.mapSizeX || pt.x < 0 || pt.y >= Map.mapSizeY || pt.y < 0) {
			return false;
		}
		return true;
	}

	public static Point<float> toIsometric(Point<int> pt) {
		Point<float> tempPt = new Point<float>();
		tempPt.x = (pt.x - pt.y) * (InitMap.tileWidth / 2);
		tempPt.y = (pt.x + pt.y) * (InitMap.tileHeight / 4);
		return tempPt;
	}

	public static Point<int> fromIsometric(Point<float> pt){
		Point<int> tempPt = new Point<int>();
		tempPt.x = (int) System.Math.Ceiling (((pt.x / (InitMap.tileWidth / 2)) + (pt.y / (InitMap.tileHeight / 4))) / 2);
		tempPt.y = (int) System.Math.Ceiling (((pt.y / (InitMap.tileHeight / 4)) - (pt.x / (InitMap.tileWidth / 2))) / 2);
		return tempPt;
	}

	public static Point<int> fromIsometricStart(Point<float> pt){
		Point<int> tempPt = new Point<int>();
		tempPt.x = (int) System.Math.Round (((pt.x / (InitMap.tileWidth / 2)) + (pt.y / (InitMap.tileHeight / 4))) / 2);
		tempPt.y = (int) System.Math.Round (((pt.y / (InitMap.tileHeight / 4)) - (pt.x / (InitMap.tileWidth / 2))) / 2);
		return tempPt;
	}

	public static Point<int> fromScreen(Vector3 screenPt) {
		Point<float> worldPt = new Point<float>();
		worldPt.x = Camera.main.ScreenToWorldPoint (screenPt).x;
		worldPt.y = Camera.main.ScreenToWorldPoint (screenPt).y;
		Point<int> mapPt = fromIsometric (worldPt);
		return mapPt;
	}
}
