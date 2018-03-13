using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Map {
	public static int mapSizeX = 10;
	public static int mapSizeY = 10;
	public static TileData[,] mapData = new TileData[mapSizeX, mapSizeY];
	public static StructureData[,] buildingData = new StructureData[mapSizeX, mapSizeY];
	public static ResourceData[,] resourceData = new ResourceData[mapSizeX, mapSizeY];
	public static int numberOfHills = 1;
	public static int numberOfLakes = 1;
	public static int numberOfForests = 3;
	public static int numberOfStones = 5;

	public static void InitMapSize (int xSize, int ySize) {
		mapSizeX = xSize;
		mapSizeY = ySize;

		mapData = new TileData[mapSizeX, mapSizeY];
		buildingData = new StructureData[mapSizeX, mapSizeY];
		resourceData = new ResourceData[mapSizeX, mapSizeY];
	}

}
