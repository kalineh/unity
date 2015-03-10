using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite;
	public int hp = 4;
	private SpriteRenderer spriteRenderer;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void DamageWall(int dmg)
	{
		spriteRenderer.sprite = dmgSprite;
		hp -= dmg;
		if (hp <= 0)
			gameObject.SetActive(false);
	}
}
