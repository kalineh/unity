using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float turnDelay = 0.2f;
	public static GameManager instance = null;
	public BoardManager boardScript;

	private int level = 3;

	public int playerFoodPoints = 100;

	private List<Enemy> enemies;
	private bool enemiesMoving;

	[HideInInspector]
	public bool playersTurn = true;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame();

	}

	void InitGame()
	{
		enemies.Clear();
		boardScript.SetupScene(level);
	}

	// Use this for initialization
	void Start () {
	
	}

	public void GameOver()
	{
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving)
			return;

		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);

		if (enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; ++i)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}









