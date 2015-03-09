using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count(int min, int max)
		{
			minimum = min;
			maximum	= max;
		}
	}

	public int Columns = 8;
	public int Rows = 8;
	public Count wallCount = new Count(5, 9);
	public Count foodCount = new Count(1, 5);

	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	public Transform boardHolder;

	private List<Vector3> gridPositions = new List<Vector3>();

	void InitializeList()
	{
		gridPositions.Clear();

		int margin = 1;

		for (int y = margin; y < Rows - margin; ++y)
		{
			for (int x = margin; x < Columns - margin; ++x)
			{
				var v = new Vector3(x, y, 0.0f);
				gridPositions.Add(v);
			}
		}
	}

	void BoardSetup()
	{
		boardHolder = new GameObject("Board").transform;

		for (int y = -1; y < Rows + 1; ++y)
		{
			for (int x = -1; x < Columns + 1; ++x)
			{
				var n = Random.Range(0, floorTiles.Length - 1);
				var t = floorTiles[n];

				if (x <= -1 || x >= Columns || y <= -1 || y >= Rows)
				{
					n = Random.Range(0, outerWallTiles.Length); 
					t = outerWallTiles[n];
				}

				var o = Instantiate(t, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;

				o.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 PopRandomPosition()
	{
		var n = Random.Range(0, gridPositions.Count);
		var p = gridPositions[n];

		gridPositions.RemoveAt(n);

		return p;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		int count = Random.Range(minimum, maximum + 1);

		for (int i = 0; i < count; ++i)
		{
			var p = PopRandomPosition();
			var n = Random.Range(0, tileArray.Length);
			var t = tileArray[n];

			Instantiate(t, p, Quaternion.identity);
		}
	}

	public void SetupScene(int level)
	{
		BoardSetup();
		InitializeList();

		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

		int enemies = (int)Mathf.Log(level, 2.0f);

		LayoutObjectAtRandom(enemyTiles, enemies, enemies);

		var p = new Vector3(Columns - 1, Rows - 1, 0.0f);

		Instantiate(exit, p, Quaternion.identity);
	}
}
