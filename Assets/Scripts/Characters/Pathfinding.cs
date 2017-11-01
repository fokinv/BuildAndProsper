using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding {
	private List<Element> openList = new List<Element>();
	private List<Element> closedList = new List<Element>();
	private Point<int> target;
	private Point<int> start;

	public class Element {
		public Point<int> coords { get; set; }
		public Element parent { get; set; }
		public int distanceFromStart { get; set; }
		public int distanceFromTarget { get; set; }
		public int estimatedCost { get; set; }

		public Element(Point<int> coords, Element parent, Point<int> target) {
			this.coords = coords;
			this.parent = parent;
			//Debug.Log("Element1");
			setDistanceAndCost (target);
			//Debug.Log("Element2");
		}

		public void update(Element newParent, Point<int> target) {
			this.parent = newParent;
			//Debug.Log("updateElement1");
			setDistanceAndCost (target);
			//Debug.Log("updateElement2");
		}

		private void setDistanceAndCost(Point<int> target) {
			if (parent != null) {
				if (isVerticalOrHorizontalMove(parent.coords, this.coords)) {
					distanceFromStart = parent.distanceFromStart + 10;
				} else {
					distanceFromStart = parent.distanceFromStart + 14;
				}
			} else {
				distanceFromStart = 0;
			}
			distanceFromTarget = (System.Math.Abs((target.x - this.coords.x)) + System.Math.Abs((target.y - this.coords.y))) * 10;
			this.estimatedCost = distanceFromStart + distanceFromTarget;
		}
	}

	public static bool isVerticalOrHorizontalMove(Point<int> start, Point<int> point2) {
		if (((point2.x == start.x + 1 || point2.x == start.x - 1) && (point2.y == start.y)) ||
			((point2.y == start.y + 1 || point2.y == start.y - 1) && (point2.x == start.x))) {
			return true;
		}
		return false;
	}

	public List<Point<int>> pathFinding(Point<int> startPt, Point<int> targetPt) {
		this.target = targetPt;
		this.start = startPt;

		if (start.x == target.x && start.y == target.y) {
			return new List<Point<int>> ();
		}

		if (target.x > Map.mapSizeX - 1) {
			target.x = Map.mapSizeX - 1;
		}
		if (target.y > Map.mapSizeY - 1) {
			target.y = Map.mapSizeY - 1;
		}
		if (target.x < 0) {
			target.x = 0;
		}
		if (target.y < 0) {
			target.y = 0;
		}

		TileData targetTile = Map.mapData [target.x, target.y];
		if (!targetTile.isWalkable) {
			return new List<Point<int>> ();
		}

		//Debug.Log("pathfinding1");
		Element currentElem = new Element (start, null, target);
		//Debug.Log ("start: " + start.x + " " + start.y);
		//Debug.Log ("target: " + target.x + " " + target.y);
		//Debug.Log ("currentElem: " + currentElem.coords.x + " " + currentElem.coords.y);

		//Debug.Log("pathfinding2");
		openList.Add (currentElem);
		bool targetNotInClosed = true;
		while (targetNotInClosed) {
			//Debug.Log("pathfinding1");
			closedList.Add (currentElem);
			while (openList.Contains (currentElem)) {
				openList.Remove (currentElem);
			}
			if (currentElem.coords.x == target.x && currentElem.coords.y == target.y) {
				targetNotInClosed = false;
			}
			addToOrUpdateOpenList (currentElem);
		//Debug.Log (openList.Count);
		//Debug.Log (closedList.Count);
			currentElem = returnMinimumCost ();
			//Debug.Log ("currentElem2: " + currentElem.coords.x + " " + currentElem.coords.y);
		}

		List<Point<int>> path =  pathToReturn();
		openList.Clear ();
		closedList.Clear ();
		target = null;
		start = null;
		return path;
	}

	private List<Point<int>> pathToReturn() {
		List<Point<int>> path = new List<Point<int>>();
		//Debug.Log ("returnPath1");
		Element currentElem = findElemInList(closedList, target);
		//Debug.Log ("returnPath2");
		bool isStart = false;
		do {
			//Debug.Log("currentElem: " + currentElem.coords.x + " " + currentElem.coords.y);
			if (currentElem.coords.x == start.x && currentElem.coords.y == start.y) {
				isStart = true;
			} //else {
				path.Add(currentElem.coords);
				currentElem = currentElem.parent;
			//}
		} while (!isStart);
		path.Reverse ();
		return path;
	}

	private void addToOrUpdateOpenList(Element currentElem) {
		List<Point<int>> surroundings = getSurroundings (currentElem.coords);
		//Debug.Log ("surroundings: " + surroundings.Count);
		for (int i=0; i < surroundings.Count; i++) {
			//Debug.Log (i);
			Point<int> temp = surroundings [i];
			//Debug.Log ("temp: " + temp.x + " " + temp.y);
			Element newElem = new Element (temp, currentElem, target);
			if (temp.x > Map.mapSizeX - 1 || temp.y > Map.mapSizeY -1 || temp.y < 0 || temp.y < 0) {
				Debug.Log ("Temp rossz: " + temp.x + " " + temp.y);
			}
			TileData pointToExamine = Map.mapData [temp.x, temp.y];
			Element oldElemClosed = findElemInList(closedList, temp);
			//Debug.Log (oldElemClosed.coords.x + " " + oldElemClosed.coords.y);
			if (pointToExamine.isWalkable && oldElemClosed == null) {
				//Debug.Log ("addToOrUpdateOpenList4");
				Element oldElemOpen = findElemInList(openList, temp);
				//Debug.Log (oldElemOpen.coords.x + " " + oldElemOpen.coords.y);
				if (oldElemOpen != null) {
					//Debug.Log ("addToOrUpdateOpenList4a");
					if (newElem.distanceFromStart < oldElemOpen.distanceFromStart) {
						oldElemOpen.update (currentElem, target);
					}
				} else {
					//Debug.Log ("addToOrUpdateOpenList4b");
					openList.Add (newElem);
				}
			}
		}
	}

	private List<Point<int>> getSurroundings(Point<int> pt) {
		List<Point<int>> list = new List<Point<int>> ();
		if (pt.x < Map.mapSizeX - 1) {
			list.Add (new Point<int>(pt.x + 1, pt.y));
		}
		if (pt.y < Map.mapSizeY - 1) {
			list.Add (new Point<int>(pt.x, pt.y + 1));
		}
		if (pt.x < Map.mapSizeX - 1 && pt.y < Map.mapSizeY - 1) {
			list.Add (new Point<int>(pt.x + 1, pt.y + 1));
		}

		if (pt.x > 0) {
			if (pt.y < Map.mapSizeY - 1) {
				list.Add (new Point<int> (pt.x - 1, pt.y + 1));
			}
			list.Add (new Point<int>(pt.x - 1, pt.y));
		}
		if (pt.y > 0) {
			list.Add (new Point<int>(pt.x, pt.y - 1));
			if (pt.x < Map.mapSizeX - 1) {
				list.Add (new Point<int> (pt.x + 1, pt.y - 1));
			}
		}
		if (pt.x > 0 && pt.y > 0) {
			list.Add (new Point<int>(pt.x - 1, pt.y - 1));
		}

		return list;
	}

	// helper functions

	private Element returnMinimumCost() {
		if (openList.Count != 0) {
			Element nextElem = null;
			//Debug.Log ("returnMinimumCost1");
			int min = System.Int32.MaxValue;
			foreach (Element e in openList) {
				//Debug.Log ("returnMinimumCost2");
				if (e.estimatedCost < min) {
					min = e.estimatedCost;
					nextElem = e;
				}
			}
			return nextElem;
		}
		return null;
	}

	private Element findElemInList(List<Element> list, Point<int> pt) {
		foreach(Element elem in list) {
			if (elem.coords.x == pt.x && elem.coords.y == pt.y) {
				return elem;
			}
		}
		return null;
	}
}
