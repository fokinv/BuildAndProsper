using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
	public static int mapSizeX = 10;
	public static int mapSizeY = 10;
	public static TileData[,] mapData = new TileData[mapSizeX, mapSizeY];
	public static TileData[,] buildingData = new TileData[mapSizeX, mapSizeY];
	public static TileData[,] resourceData = new TileData[mapSizeX, mapSizeY];
	public static int numberOfHills = 0;
	public static int numberOfLakes = 0;

}
