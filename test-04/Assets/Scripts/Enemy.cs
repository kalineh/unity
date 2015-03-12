using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public int playerDamage;

	private Animator animator;
	private Transform target;

	private bool skipMove;

	// Use this for initialization
	protected override void Start ()
	{
		GameManager.instance.AddEnemyToList(this);

		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;

		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected override void AttemptMove<T>(int xd, int yd)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}

		base.AttemptMove<T>(xd, yd);

		skipMove = true;
	}

	public void MoveEnemy()
	{
		var ofs = target.position - transform.position;

		int xd = (int)Mathf.Sign(ofs.x);
		int yd = (int)Mathf.Sign(ofs.y);

		if (xd != 0)
			yd = 0;

		AttemptMove<Player>(xd, yd);
	}

	protected override void OnCantMove<T>(T component)
	{
		Player hitPlayer = component as Player;

		animator.SetTrigger("enemyAttack");

		hitPlayer.LoseFood(playerDamage);
	}
}








