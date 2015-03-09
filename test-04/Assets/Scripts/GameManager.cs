using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public BoardManager boardScript;

	private int level = 3;

	void Awake()
	{
		boardScript = GetComponent<BoardManager>();
		InitGame();

	}

	void InitGame()
	{
		boardScript.SetupScene(level);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
