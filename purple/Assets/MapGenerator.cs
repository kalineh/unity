﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;


	// Use this for initialization
	void Start () {
		GenerateMap();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap();
		}
	}

	public void GenerateMap()
	{
		Debug.Log ("GenerateMap()");

		map = new int[width, height];

		RandomFillMap ();

		for (int i = 0; i < 5; ++i)
		{
			SmoothMap();
		}

		ProcessMap ();

		int borderSize = 5;
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength(0); ++x) {
			for (int y = 0; y < borderedMap.GetLength(1); ++y) {
				if (x >= borderSize && x < width + borderSize &&
				    y >= borderSize && y < height + borderSize)
				{
					borderedMap[x,y] = map[x-borderSize, y-borderSize];
				}
				else
				{
					borderedMap[x,y] = 1;
				}
			}
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator> ();
		meshGen.GenerateMesh(borderedMap, 1.0f);
	}

	void ProcessMap()
	{
		List<List<Coord>> wallRegions = GetRegions (1);

		int wallThresholdSize = 50;

		foreach (List<Coord> wallRegion in wallRegions)
		{
			if (wallRegion.Count < wallThresholdSize)
			{
				foreach (Coord tile in wallRegion)
				{
					map[tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions (0);
		int roomThresholdSize = 20;
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Coord> roomRegion in roomRegions)
		{
			if (roomRegion.Count < roomThresholdSize)
			{
				foreach (Coord tile in roomRegion)
				{
					map[tile.tileX, tile.tileY] = 1;
				}
			}
			else
			{
				survivingRooms.Add (new Room(roomRegion, map));
			}
		}

		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;

		ConnectClosestRooms (survivingRooms, false);
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom)
	{
		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();

		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add (room);
				} else {
					roomListA.Add (room);
				}
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom)
			{
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0)
				{
					continue;
				}
			}

			foreach (Room roomB in roomListB)
			{
				if (roomA == roomB)
				{
					continue;
				}

				if (roomA.IsConnected(roomB))
				{
					continue;
				}

				if (roomA.IsConnected(roomB))
				{
					possibleConnectionFound = false;
					break;
				}

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; ++tileIndexA)
				{
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; ++tileIndexB)
					{
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms =
							(int)(
							Mathf.Pow (tileA.tileX - tileB.tileX, 2.0f) +
							Mathf.Pow (tileA.tileY - tileB.tileY, 2.0f));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
						{
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
					
				}
			}

			if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms(allRooms, true);
		}
	}

	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
	{
		Room.ConnectRoom (roomA, roomB);

		Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100.0f);
	}

	Vector3 CoordToWorldPoint(Coord tile)
	{
		return new Vector3 (-width / 2.0f + tile.tileX, 2.0f, -height / 2.0f + tile.tileY);
	}

	List<List<Coord>> GetRegions(int tileType)
	{
		List<List<Coord>> regions = new List<List<Coord>> ();

		int[,] mapFlags = new int[width, height];

		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				if (mapFlags [x, y] == 0 && map [x, y] == tileType) {
					List<Coord> newRegion = GetRegionTiles (x, y);
					regions.Add (newRegion);

					foreach (Coord c in newRegion)
					{
						mapFlags[c.tileX, c.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles(int startX, int startY)
	{
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[width, height];
		int tileType = map[startX, startY];

		Queue<Coord> queue = new Queue<Coord>();

		queue.Enqueue(new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add (tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; ++x)
			{
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; ++y)
				{
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX))
					{
						if (mapFlags[x,y] == 0 && map[x,y] == tileType)
						{
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}

		return tiles;
	}

	bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	void RandomFillMap()
	{
		if (useRandomSeed || true) {
			seed = Time.time.ToString ();
		}	

		var r = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (x == 0 || x == width-1 || y == 0 | y == height-1 )
					map[x,y] = 1;
				else
					map[x,y] = (r.Next (0,100) < randomFillPercent ) ? 1 : 0;
			}
		}
	}

	void SmoothMap()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				int neighbour = CountWalls(x,y);

				if (neighbour > 4 )
					map[x,y] = 1;
				else if (neighbour < 4)
					map[x,y] = 0;
			}
		}
	}

	int CountWalls(int x, int y)
	{
		int wallCount = 0;

		for (int nx = x - 1; nx <= x + 1; ++nx)
		{
			for (int ny = y - 1; ny <= y + 1; ++ny)
			{
				if (IsInMapRange(nx, ny))
				{
					if (nx != x || ny != y )
					{
						wallCount += map[nx,ny];
					}
				}
				else{
					wallCount++;
				}
			}
		}

		return wallCount;
	}

	struct Coord
	{
		public int tileX;
		public int tileY;

		public Coord(int x, int y)
		{
			tileX = x;
			tileY = y;
		}
	};

	class Room : IComparable<Room>
	{
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room()
		{
		}

		public Room(List<Coord> roomTiles, int[,] map)
		{
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();
			edgeTiles = new List<Coord>();

			foreach (Coord tile in tiles)
			{
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; ++x)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; ++y)
					{
						if (x == tile.tileX || y == tile.tileY)
						{
							if (map[x,y] == 1)
							{
								edgeTiles.Add (tile);
							}
						}
					}
				}
			}
		}

		public static void ConnectRoom(Room a, Room b)
		{
			if (a.isAccessibleFromMainRoom) {
				b.SetAccessibleFromMainRoom ();
			} else if (b.isAccessibleFromMainRoom) {
				a.SetAccessibleFromMainRoom();
			}

			a.connectedRooms.Add (b);
			b.connectedRooms.Add (a);

		}

		public bool IsConnected(Room other)
		{
			return connectedRooms.Contains (other);
		}

		public int CompareTo(Room other)
		{
			return other.roomSize.CompareTo (roomSize);
		}
	
		public void SetAccessibleFromMainRoom()
		{
			if (!isAccessibleFromMainRoom)
			{
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms)
				{
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}
	}
}
