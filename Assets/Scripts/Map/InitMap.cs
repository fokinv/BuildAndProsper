using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMap : MonoBehaviour {
	public static TileData[,] mapData;
	public Transform groundTile;
	public static float tileWidth = 0.64f;
	public static float tileHeight = 0.64f;

	void Start () {
		//mapData = new TileData[mapSizeX, mapSizeY];

		for (int i = Map.mapSizeY - 1; i >= 0; i--) {
			for (int j = Map.mapSizeX - 1; j >= 0; j--) {
				Map.mapData [j, i] = new TileData ();
			}
		}

		createHillsAndLakes ();

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

				/*Debug.Log ("base: " + j + " " + i);
				Debug.Log ("iso: " + iso_pt.x + " " + iso_pt.y);*/
				//Debug.Log ("x: " + j + " y: " + i);
				//Point<int> map = Point<int>.fromIsometric (iso_pt);

				/*Debug.Log(map_x + " " + map_y);
				Debug.Log(iso_pt.x + " " + iso_pt.y);
				Debug.Log("---------------------------------------");*/
				/*Debug.Log ("Map: " + map_x + "," + map_y + "Screen: " + iso_x + "," + iso_y);
				Point debug_point = fromIsometric(iso_pt);
				Debug.Log ("Map: " + map_x + "," + map_y + "Map2: " + debug_point.x + "," + debug_point.y);
				Debug.Log ("-------------------------------------");*/
				placeTile (groundTile, iso_pt, Map.mapData [j, i]);
				//Debug.Log (mapData [j, i].tile);
			}
		}
	}

	void Update () {
		
	}

	// Places the tile in the world map
	private void placeTile(Transform tile, Point<float> iso_pt, TileData tileData) {
		Transform tileObject = Instantiate (tile, new Vector3 (iso_pt.x, iso_pt.y, 0), Quaternion.identity) as Transform;
		switch (tileData.tileType) {
		case TileData.TileType.Lake:
		case TileData.TileType.Ground:
			{
				tileObject.GetComponent<Renderer>().sortingOrder = 0;
				break;
			}
		case TileData.TileType.Hill:
			{
				tileObject.GetComponent<Renderer>().sortingOrder = 0;
				break;
			}
		}
		tileObject.parent = transform;
		tileData.initTile (tileObject);
		//Debug.Log ("x: " + iso_pt.x + " y: " + iso_pt.y);
		//Debug.Log("------------------");
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
			//Debug.Log (middle.x + " " + middle.y);

			Environment newMountain = new Environment (middle, radius, type);
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
			//Debug.Log (middle.x + " " + middle.y);

			Environment newMountain = new Environment (middle, radius, type);
		}
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

	/*public static Point<int>[] getSurroundings(Point<int> pt) {

	}*/
}
