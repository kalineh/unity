﻿using UnityEngine;
using System.Collections;

// 1..2
// .  .
// 3..4

public class MeshGenerator : MonoBehaviour {

	public SquareGrid squareGrid;

	public void GenerateMesh(int[,] map, float squareSize)
	{
		squareGrid = new SquareGrid (map, squareSize);
	}

	public void OnDrawGizmos()
	{
		if (squareGrid != null) {
			for (int x = 0; x < squareGrid.squares.GetLength(0); ++x) {
				for (int y = 0; y < squareGrid.squares.GetLength(1); ++y) {
					
					Gizmos.color = squareGrid.squares [x, y].topLeft.active ? Color.blue : Color.red;
					Gizmos.DrawCube (squareGrid.squares [x, y].topLeft.position, Vector3.one * 0.4f);

					Gizmos.color = squareGrid.squares [x, y].topRight.active ? Color.blue : Color.red;
					Gizmos.DrawCube (squareGrid.squares [x, y].topRight.position, Vector3.one * 0.4f);

					Gizmos.color = squareGrid.squares [x, y].bottomRight.active ? Color.blue : Color.red;
					Gizmos.DrawCube (squareGrid.squares [x, y].bottomRight.position, Vector3.one * 0.4f);

					Gizmos.color = squareGrid.squares [x, y].bottomLeft.active ? Color.blue : Color.red;
					Gizmos.DrawCube (squareGrid.squares [x, y].bottomLeft.position, Vector3.one * 0.4f);

					Gizmos.color = Color.grey;
					Gizmos.DrawCube (squareGrid.squares [x, y].centerTop.position, Vector3.one * 0.15f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centerRight.position, Vector3.one * 0.15f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centerBottom.position, Vector3.one * 0.15f);
					Gizmos.DrawCube (squareGrid.squares [x, y].centerLeft.position, Vector3.one * 0.15f);
				}
			}
		}
	}

	public class SquareGrid
	{
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize)
		{
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

			for (int x = 0; x < nodeCountX; ++x)
			{
				for (int y = 0; y < nodeCountY; ++y)
				{
					Vector3 p = new Vector3(-mapWidth / 2.0f + x * squareSize + squareSize / 2.0f, 0.0f, -mapHeight / 2.0f + y * squareSize + squareSize / 2.0f);
					controlNodes[x,y] = new ControlNode(p, map[x,y] != 0, squareSize);
				}
			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];

			for (int x = 0; x < nodeCountX-1; ++x)
			{
				for (int y = 0; y < nodeCountY-1; ++y)
				{
					squares[x,y] = new Square(
						controlNodes[x,y+1],
						controlNodes[x+1,y+1],
						controlNodes[x+1,y],
						controlNodes[x,y]);
				}
			}
		}
	}

	public class Square
	{
		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centerTop, centerRight, centerBottom, centerLeft;

		public Square(ControlNode tl, ControlNode tr, ControlNode br, ControlNode bl)
		{
			topLeft = tl;
			topRight = tr;
			bottomRight = br;
			bottomLeft = bl;

			centerTop = topLeft.right;
			centerRight = bottomRight.above;
			centerBottom = bottomLeft.right;
			centerLeft = bottomLeft.above;
		}
	}

	public class Node
	{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 p)
		{
			position = p;
		}
	}

	public class ControlNode : Node
	{
		public bool active;
		public Node above;
		public Node right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
		{
			active = _active;
			above = new Node(_pos + Vector3.forward * squareSize * 0.5f);
			right = new Node(_pos + Vector3.right * squareSize * 0.5f);
		}
	}

	// Use this for initialization
	void Start () {
	
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
}
