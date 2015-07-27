using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 1..2
// .  .
// 3..4

public class MeshGenerator : MonoBehaviour {

	public SquareGrid squareGrid;
	List<Vector3> vertices;
	List<int> triangles;

	public void GenerateMesh(int[,] map, float squareSize)
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();

		squareGrid = new SquareGrid (map, squareSize);

		for (int x = 0; x < squareGrid.squares.GetLength(0); ++x) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); ++y) {
				TriangulateSquare(squareGrid.squares[x,y]);
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
	
	void TriangulateSquare(Square square) {
		switch (square.configuration) {
		case 0:
			break;
			
			// 1 points:
		case 1:
			MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
			break;
		case 2:
			MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
			break;
		case 4:
			MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
			break;
		case 8:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
			break;
			
			// 2 points:
		case 3:
			MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 6:
			MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
			break;
		case 9:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
			break;
		case 5:
			MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
			break;
		case 10:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;
			
			// 3 point:
		case 7:
			MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 11:
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;
			
			// 4 point:
		case 15:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			break;
		}
	}

	void MeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if (points.Length >= 3) {
			CreateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length >= 4) {
			CreateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length >= 5) {
			CreateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length >= 6) {
			CreateTriangle(points[0], points[4], points[5]);
		}
	}

	void AssignVertices(Node[] points)
	{
		for (int i = 0; i < points.Length; ++i)
		{
			if (points[i].vertexIndex == -1)
			{
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c)
	{
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);
	}

	public void OnDrawGizmos()
	{
		return;
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
		public int configuration;

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

			if (topLeft.active) configuration |= 0x0008;
			if (topRight.active) configuration |= 0x0004;
			if (bottomRight.active) configuration |= 0x0002;
			if (bottomLeft.active) configuration |= 0x0001;
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
