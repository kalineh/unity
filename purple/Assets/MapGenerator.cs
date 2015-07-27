using UnityEngine;
using System.Collections;

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
				if (nx >= 0 && nx < width && ny >= 0 && ny < height)
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

	void OnDrawGizmos()
	{
		if (map == null) {
			GenerateMap();
		}

		if (Input.GetMouseButtonDown (0))
		{
			Debug.Log("arasdfasdf");
			GenerateMap();
		}

		return;
		if (map != null)
		{
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					Gizmos.color = (map[x,y] == 1) ? Color.black : Color.white;
					Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2.0f + y + 0.5f);
					Vector3 size = new Vector3(1,1,1);
					Gizmos.matrix = this.gameObject.transform.localToWorldMatrix;
					Gizmos.DrawCube(pos, size);
				}
			}
		}
	}
}
