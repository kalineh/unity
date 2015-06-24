using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;



public class Board
	: MonoBehaviour
{
	[Range(1, 64)]
	public int DimensionX = 32;

	[Range(1, 64)]
	public int DimensionY = 32;

	public List<Gem> GemPool;

	private List<Gem> gems = new List<Gem>();

	// Use this for initialization
	void Start()
	{
		// create board
		// create pieces from prefabs
		// input handler
		// 
		var scale = 1.0f;

		for (int y = 0; y < DimensionY; ++y)
		{
			for (int x = 0; x < DimensionX; ++x)
			{
				var gem = CreateRandomGem();
				var xy = new Vector2(x, y) * scale;

				gem.transform.position = xy;
				gem.transform.localScale = new Vector2(scale, scale);

				gems.Add(gem);
			}
		}
	}

	Gem CreateRandomGem()
	{
		var index = Random.Range(0, GemPool.Count);
		var source = GemPool[index];
		var gem = Instantiate(source);

		return gem;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
