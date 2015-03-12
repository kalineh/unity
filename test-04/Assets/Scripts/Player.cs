using UnityEngine;
using System.Collections;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1.0f;

	private Animator animator;
	private int food = 0;

	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;

		base.Start();
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	private void CheckIfGameOver()
	{
		if (food <= 0)
			GameManager.instance.GameOver();
	}

	protected override void AttemptMove<T>(int xd, int yd)
	{
		food--;

		base.AttemptMove<T>(xd, yd);

		RaycastHit2D hit;

		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int h = (int)Input.GetAxisRaw("Horizontal");
		int v = (int)Input.GetAxisRaw("Vertical");

		if (h != 0)
			v = 0;
		                             
		if (h != 0 || v != 0)
		{
			AttemptMove<Wall>(h, v);
		}
	}

	protected override void OnCantMove<T>(T component)
	{
		Wall hitWall = component as Wall;

		hitWall.DamageWall(wallDamage);

		animator.SetTrigger("animatorChop");
	}

	private void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void LoseFood(int loss)
	{
		animator.SetTrigger("playerHit");

		food -= loss;

		CheckIfGameOver();
	}

	private void OnTriggerEnter2D(Collider2D rhs)
	{
		if (rhs.tag == "Exit")
		{
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (rhs.tag == "Food")
		{
			food += pointsPerFood;
			rhs.gameObject.SetActive(false);
		}
		else if (rhs.tag == "Soda")
		{
			food += pointsPerSoda;
			rhs.gameObject.SetActive(false);
		}
	}
}






















