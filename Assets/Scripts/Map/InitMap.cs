using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMap : MonoBehaviour {
	public static TileData[,] mapData;
	public Transform groundTile;
	public static float tileWidth = 0.64f;
	public static float tileHeight = 0.64f;

	void Start () {
		for (int i = Map.mapSizeY - 1; i >= 0; i--) {
			for (int j = Map.mapSizeX - 1; j >= 0; j--) {
				Map.mapData [j, i] = new TileData ();
			}
		}

		createHillsAndLakes ();
		createForests ();

		for (int i = Map.mapSizeY-1; i >= 0; i--) {
			for (int j = Map.mapSizeX-1; j >= 0; j--) {
				switch (Map.mapData [j, i].tileType) {
				case TileData.TileType.Ground:
					{
						string path = "Prefabs/Ground/grass" + Map.mapData [j, i].prefabID;
						loadTile (path);
						break;
					}
				case TileData.TileType.Hill:
					{
						string path = "Prefabs/Hill/hill" + Map.mapData [j, i].prefabID;
						loadTile (path);
						break;
					}
				case TileData.TileType.Lake:
					{
						string path = "Prefabs/Lake/water" + Map.mapData [j, i].prefabID;
						loadTile (path);
						break;
					}
				}
				Point<float> iso_pt = Point<float>.toIsometric (new Point<int> (j, i));
				placeTile (groundTile, iso_pt, Map.mapData [j, i]);
				if (Map.resourceData[j,i] != null) {
					switch (Map.resourceData [j, i].tileType) {
						case TileData.TileType.Forest: 
						{
							string path = "Prefabs/" + Map.resourceData [j, i].tileType.ToString () + "/Tree" + Map.resourceData [j, i].prefabID;
							loadTile (path);
							break;
						}
					case TileData.TileType.Stone: 
						{
							string path = "Prefabs/" + Map.resourceData [j, i].tileType.ToString () + "/Tree" + Map.resourceData [j, i].prefabID;
							loadTile (path);
							break;
						}
					}
					placeTile (groundTile, iso_pt, Map.resourceData [j, i]);
				}
			}
		}
	}

	// Places the tile in the world map
	private void placeTile(Transform tile, Point<float> iso_pt, TileData tileData) {
		Transform tileObject = Instantiate (tile, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;

		switch (tileData.tileType) {
		case TileData.TileType.Lake:
		case TileData.TileType.Ground:
		case TileData.TileType.Hill:
			{
				tileObject.parent = transform;
				tileObject.GetComponent<Renderer>().sortingOrder = 0;
				tileObject.tag = "Ground";
				break;
			}
		case TileData.TileType.Forest:
			{
				tileObject.GetComponent<Renderer>().sortingOrder = 1;
				tileObject.parent = GameObject.Find ("ResourcesObject").transform;
				tileObject.tag = "Resource";
				break;
			}
		}
		tileData.initTile (tileObject);
	}


	private void loadTile(string path) {
		groundTile = Resources.Load <Transform> (path);
		if (!groundTile)
			Debug.LogWarning ("Unable to find TilePrefab in your Resources folder.");
	}

	// Create hill and lake blocks at random points in the map
	private void createHillsAndLakes() {
		for (int i = 0; i < Map.numberOfHills; i++) {
			int radius = Random.Range (0, 5);
			TileData.TileType type = TileData.TileType.Hill;
			bool isClear = false;
			Point<int> middle = new Point<int>();

			while (!isClear) {
				middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
				isClear = checkRadius (middle, radius, type);
			}

			Environment.createEnvironment(middle, radius, type);
		}

		for (int i = 0; i < Map.numberOfLakes; i++) {
			int radius = Random.Range (0, 5);
			TileData.TileType type = TileData.TileType.Lake;
			bool isClear = false;
			Point<int> middle = new Point<int>();

			while (!isClear) {
				middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
				isClear = checkRadius (middle, radius, type);
			}

			Environment.createEnvironment(middle, radius, type);
		}
	}

	private void createForests() {
		for (int i = 0; i < Map.numberOfForests; i++) {
			int radius = Random.Range (0, 5);
			TileData.TileType type = TileData.TileType.Forest;
			bool isClear = false;
			Point<int> middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));

			/*while (!isClear) {
				middle = new Point<int> (Random.Range (5, Map.mapSizeX - 5), Random.Range (5, Map.mapSizeY - 5));
				radius = Random.Range (0, 5);
				isClear = canPlaceForest (middle, radius);
			}*/

			Environment.createEnvironment(middle, radius, type);
		}
	}

	private bool canPlaceForest(Point<int> middle, int radius) {
		for (int y = middle.y + radius; y >= middle.y - radius; y--) {
			for (int x = middle.x + radius; x >= middle.x - radius; x--) {
				if (Map.mapData [x, y].tileType == TileData.TileType.Ground || (Map.mapData [x, y].tileType == TileData.TileType.Hill && Map.mapData [x, y].prefabID == 1)) {
					continue;
				} else {
					return false;
				}
			}
		}
		return true;
	}

	private bool checkRadius(Point<int> middle, int radius, TileData.TileType type) {
		for (int y = middle.y + radius; y >= middle.y - radius; y--) {
			for (int x = middle.x + radius; x >= middle.x - radius; x--) {
				if (Map.mapData [x, y].isSameEnvironment(type)) {
					return false;
				}
			}
		}
		return true;
	}
}
