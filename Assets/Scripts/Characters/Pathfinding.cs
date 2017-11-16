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
			setDistanceAndCost (target);
		}

		public void update(Element newParent, Point<int> target) {
			this.parent = newParent;
			setDistanceAndCost (target);
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
			//return new List<Point<int>> ();
			target = newTarget ();
		}

		Element currentElem = new Element (start, null, target);

		openList.Add (currentElem);
		bool targetNotInClosed = true;
		while (targetNotInClosed) {
			closedList.Add (currentElem);
			while (openList.Contains (currentElem)) {
				openList.Remove (currentElem);
			}
			if (currentElem.coords.x == target.x && currentElem.coords.y == target.y) {
				targetNotInClosed = false;
			}
			addToOrUpdateOpenList (currentElem);
			currentElem = returnMinimumCost ();
		}

		List<Point<int>> path =  pathToReturn();
		openList.Clear ();
		closedList.Clear ();
		target = null;
		start = null;
		return path;
	}

	private Point<int> newTarget() {
		bool foundNewTarget = false;
		bool incRadius = true;
		int radius = 1;
		int minimumDistance = System.Int32.MaxValue;
		Point<int> tempTarget = new Point<int> ();
		while (!foundNewTarget) {
			for(int y = target.y - radius; y <= target.y + radius; y++) {
				for (int x = target.x - radius; x <= target.x + radius; x++) {
					if (x > Map.mapSizeX - 1 || y > Map.mapSizeY - 1 || x < 0 || y < 0) {
						continue;
					}
					if (!Map.mapData [x, y].isWalkable) {
						continue;
					}
					incRadius = false;
					int distanceFromTarget = (System.Math.Abs ((x - target.x)) + System.Math.Abs ((y - target.y))) * 10;
					if (distanceFromTarget < minimumDistance) {
						minimumDistance = distanceFromTarget;
						tempTarget = new Point<int> (x, y);
					}
				}
			}
			if (incRadius) {
				radius++;
			} else {
				foundNewTarget = true;
			}
		}
		return tempTarget;
	}

	private List<Point<int>> pathToReturn() {
		List<Point<int>> path = new List<Point<int>>();
		Element currentElem = findElemInList(closedList, target);
		bool isStart = false;
		do {
			if (currentElem.coords.x == start.x && currentElem.coords.y == start.y) {
				isStart = true;
			}
			path.Add(currentElem.coords);
			currentElem = currentElem.parent;
		} while (!isStart);
		path.Reverse ();
		return path;
	}

	private void addToOrUpdateOpenList(Element currentElem) {
		List<Point<int>> surroundings = getSurroundings (currentElem.coords);
		for (int i=0; i < surroundings.Count; i++) {
			Point<int> temp = surroundings [i];
			Element newElem = new Element (temp, currentElem, target);
			if (temp.x > Map.mapSizeX - 1 || temp.y > Map.mapSizeY -1 || temp.y < 0 || temp.y < 0) {
				Debug.Log ("Temp rossz: " + temp.x + " " + temp.y);
			}
			TileData pointToExamine = Map.mapData [temp.x, temp.y];
			Element oldElemClosed = findElemInList(closedList, temp);
			if (pointToExamine.isWalkable && oldElemClosed == null) {
				Element oldElemOpen = findElemInList(openList, temp);
				if (oldElemOpen != null) {
					if (newElem.distanceFromStart < oldElemOpen.distanceFromStart) {
						oldElemOpen.update (currentElem, target);
					}
				} else {
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
			int min = System.Int32.MaxValue;
			foreach (Element e in openList) {
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
