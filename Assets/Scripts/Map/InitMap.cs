using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMap : MonoBehaviour {
	private Transform resourcesObject;

	public static TileData[,] mapData;
	public Transform tileToLoad;
	public static float tileWidth = 0.64f;
	public static float tileHeight = 0.64f;

	void Start () {
		resourcesObject = GameObject.Find ("ResourcesObject").transform;
		Debug.Log (Map.mapSizeX + " " + Map.mapSizeY);

		for (int i = Map.mapSizeY - 1; i >= 0; i--) {
			for (int j = Map.mapSizeX - 1; j >= 0; j--) {
				Map.mapData [j, i] = new TileData ();
			}
		}

		CreateHillsAndLakes ();
		CreateForests ();
		CreateStones ();

		int startOrder = 0;
		int currentOrder = 0;
		for (int i = Map.mapSizeY-1; i >= 0; i--) {
			currentOrder = startOrder;
			for (int j = Map.mapSizeX-1; j >= 0; j--) {
				string path = "";
				Point<float> iso_pt = Point<float>.ToIsometric (new Point<int> (j, i));
				Transform tileObject = null;

				switch (Map.mapData [j, i].tileType) {
				case TileData.TileType.Ground:
					{
						path = "Prefabs/Ground/grass" + Map.mapData [j, i].prefabID;
						LoadTile (path);
						break;
					}
				case TileData.TileType.Hill:
					{
						path = "Prefabs/Hill/hill" + Map.mapData [j, i].prefabID;
						LoadTile (path);
						break;
					}
				case TileData.TileType.Lake:
					{
						path = "Prefabs/Lake/water" + Map.mapData [j, i].prefabID;
						LoadTile (path);
						break;
					}
				}

				tileObject = PlaceTile (tileToLoad, iso_pt/*, Map.mapData [j, i]*/);
				tileObject.GetComponent<Renderer>().sortingOrder = 0;
				Map.mapData [j, i].InitTile (tileObject);

				if (Map.resourceData [j, i] != null) {
					//create resource
					ResourceData resource = Map.resourceData[j, i];
					switch (Map.resourceData [j, i].resourceType) {
					case ResourceData.ResourceType.Stone:
						int stoneAmount = Random.Range (0, 101);
						if (stoneAmount <= 100 && stoneAmount > 75) {
							path = "Prefabs/Stone/StoneLarge" + Random.Range (1, 3 /*Number of Stone types*/);
						} else if (stoneAmount <= 75 && stoneAmount > 50) {
							path = "Prefabs/Stone/StoneMedium" + Random.Range (1, 9 /*Number of Stone types*/);
						} else if (stoneAmount <= 50 && stoneAmount > 25) {
							path = "Prefabs/Stone/StoneMidSmall" + Random.Range (1, 3 /*Number of Stone types*/);
						} else {
							path = "Prefabs/Stone/StoneSmall" + Random.Range (1, 3 /*Number of Stone types*/);
						}

						LoadTile (path);
						tileObject = PlaceTile (tileToLoad, iso_pt);
						//Transform tileObject = Instantiate (tileToLoad, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
						tileObject.GetComponent<Renderer>().sortingOrder = currentOrder + 1;
						tileObject.parent = resourcesObject;
						tileObject.tag = "Resource";
						tileObject.gameObject.AddComponent<Stone> ();
						//ResourceData stoneData = new ResourceData (ResourceData.ResourceType.Stone, tileObject);
						resource.InitTile(tileObject);
						resource.script.amountLeft = stoneAmount;
						break;
					case ResourceData.ResourceType.Wood:
						path = "Prefabs/Wood/Tree" + Random.Range (1, 2 /*Number of tree types*/);
						LoadTile (path);
						tileObject = PlaceTile (tileToLoad, iso_pt);
						//Transform tileObject = Instantiate (tileToLoad, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
						tileObject.GetComponent<Renderer> ().sortingOrder = currentOrder + 1;
						tileObject.parent = resourcesObject;
						tileObject.tag = "Resource";
						resource.InitTile(tileObject);
						break;
					}
				}
				currentOrder++;
			}
			startOrder++;
		}
	}

	// Places the tile in the world map
	private Transform PlaceTile(Transform tile, Point<float> iso_pt/*, TileData tileData*/) {
		Transform tileObject = Instantiate (tile, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
		tileObject.parent = transform;
		//tileObject.GetComponent<Renderer>().sortingOrder = 0;
		//tileObject.tag = "Ground";
		//tileData.InitTile (tileObject);
		return tileObject;
	}


	private void LoadTile(string path) {
		tileToLoad = null;
		tileToLoad = Resources.Load <Transform> (path);
		if (!tileToLoad)
			Debug.LogWarning ("Unable to find TilePrefab in your Resources folder.");
	}

	// Create hill and lake blocks at random points in the map
	private void CreateHillsAndLakes() {
		for (int i = 0; i < Map.numberOfHills; i++) {
			int radius = Random.Range (0, 5);
			TileData.TileType type = TileData.TileType.Hill;
			bool isClear = false;
			Point<int> middle = new Point<int>();

			while (!isClear) {
				middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
				isClear = CheckRadius (middle, radius, type);
			}

			Environment.CreateEnvironment(middle, radius, type);
		}

		for (int i = 0; i < Map.numberOfLakes; i++) {
			int radius = Random.Range (0, 5);
			TileData.TileType type = TileData.TileType.Lake;
			bool isClear = false;
			Point<int> middle = new Point<int>();

			while (!isClear) {
				middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
				isClear = CheckRadius (middle, radius, type);
			}

			Environment.CreateEnvironment(middle, radius, type);
		}
	}

	private void CreateForests() {
		for (int i = 0; i < Map.numberOfForests; i++) {
			int maxRadius = Map.mapSizeX < Map.mapSizeY ? Map.mapSizeX / 22 : Map.mapSizeY / 2;
			int radius = Random.Range (0, maxRadius );
			//TileData.TileType type = TileData.TileType.Forest;
			bool isClear = false;
			Point<int> middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
			Point<int> buttomLeft = new Point<int> (middle.x - radius, middle.y - radius);
			Point<int> topRight = new Point<int> (middle.x + radius, middle.y + radius);

			for (int y = topRight.y; y >= buttomLeft.y; y--) {
				for (int x = topRight.x; x >= buttomLeft.x; x--) {
					if (Map.mapData [x, y].tileType == TileData.TileType.Ground && Map.resourceData [x, y] == null) {
						Map.mapData [x, y].isWalkable = false;
						//string path = "Prefabs/Wood/Tree" + Random.Range (1, 2 /*Number of tree types*/);
						/*LoadTile (path);
						Point<float> iso_pt = Point<float>.ToIsometric (new Point<int> (x, y));
						Transform tileObject = Instantiate (tileToLoad, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
						tileObject.GetComponent<Renderer>().sortingOrder = 1;
						tileObject.parent = resourcesObject;
						tileObject.tag = "Resource";*/
						Map.resourceData [x, y] = new ResourceData (ResourceData.ResourceType.Wood, /*tileObject*/ null);
					}
				}
			}

		}
	}

	private void CreateStones() {
		for (int i = 0; i < Map.numberOfStones; i++) {
			Point<int> stonePoint = new Point<int> (Random.Range (0, Map.mapSizeX - 1), Random.Range (0, Map.mapSizeY - 1));

			if (Map.mapData [stonePoint.x, stonePoint.y].tileType == TileData.TileType.Ground && Map.resourceData [stonePoint.x, stonePoint.y] == null) {
				//int stoneAmount = Random.Range (0, 101);
				//string path = "";
				//if (stoneAmount <= 100 && stoneAmount > 75) {
				//	path = "Prefabs/Stone/StoneLarge" + Random.Range (1, 3 /*Number of Stone types*/);
				//} else if (stoneAmount <= 75 && stoneAmount > 50) {
				//	path = "Prefabs/Stone/StoneMedium" + Random.Range (1, 9 /*Number of Stone types*/);
				//} else if (stoneAmount <= 50 && stoneAmount > 25) {
				//	path = "Prefabs/Stone/StoneMidSmall" + Random.Range (1, 3 /*Number of Stone types*/);
				//} else {
				//	path = "Prefabs/Stone/StoneSmall" + Random.Range (1, 3 /*Number of Stone types*/);
				//}

				Map.mapData [stonePoint.x, stonePoint.y].isWalkable = false;

				/*LoadTile (path);
				Point<float> iso_pt = Point<float>.ToIsometric (new Point<int> (stonePoint.x, stonePoint.y));
				Transform tileObject = Instantiate (tileToLoad, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
				tileObject.GetComponent<Renderer>().sortingOrder = 1;
				tileObject.parent = resourcesObject;
				tileObject.tag = "Resource";
				tileObject.gameObject.AddComponent<Stone> ();
				ResourceData stoneData = new ResourceData (ResourceData.ResourceType.Stone, tileObject);
				stoneData.script.amountLeft = stoneAmount;*/
				Map.resourceData [stonePoint.x, stonePoint.y] = /*stoneData*/ new ResourceData (ResourceData.ResourceType.Stone, null);
			}
		}
	}

	private bool CheckRadius(Point<int> middle, int radius, TileData.TileType type) {
		for (int y = middle.y + radius; y >= middle.y - radius; y--) {
			for (int x = middle.x + radius; x >= middle.x - radius; x--) {
				if (Map.mapData [x, y] != null) {
					if (Map.mapData [x, y].IsSameEnvironment (type))
						return false;
				}
			}
		}
		return true;
	}
}
