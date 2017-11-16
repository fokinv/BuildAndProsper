using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
	public static int mapSizeX = 20;
	public static int mapSizeY = 20;
	public static TileData[,] mapData = new TileData[mapSizeX, mapSizeY];
	public static TileData[,] buildingData = new TileData[mapSizeX, mapSizeY];
	public static TileData[,] resourceData = new TileData[mapSizeX, mapSizeY];
	public static int numberOfHills = 1;
	public static int numberOfLakes = 1;
	public static int numberOfForests = 3;

}
