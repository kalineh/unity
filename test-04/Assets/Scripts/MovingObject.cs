using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2d;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1.0f / moveTime;
	}

	protected IEnumerator SmoothMovement(Vector3 end)
	{
		var remain = (transform.position - end).sqrMagnitude;

		while (remain > float.Epsilon)
		{
			var a = rb2d.position;
			var b = end;
			var t = inverseMoveTime * Time.deltaTime;
			var p = Vector3.MoveTowards(a, b, t);

			rb2d.MovePosition(p);

			remain = (transform.position - end).sqrMagnitude;

			yield return null;
		}
	}

	protected bool Move(int xd, int yd, out RaycastHit2D hit)
	{
		Vector2 a = transform.position;
		Vector2 b = a + new Vector2(xd, yd);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast(a, b);
		boxCollider.enabled = true;

		if (hit.transform == null)
		{
			StartCoroutine(SmoothMovement(b));
			return true;
		}

		return false;
	}

	protected virtual void AttemptMove<T>(int xd, int yd)
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move(xd, yd, out hit);

		if (hit.transform == null)
			return;

		var hc = hit.transform.GetComponent<T>();

 		if (!canMove && hc != null)
		{
			OnCantMove(hc);
		}
	}

	protected abstract void OnCantMove<T>(T component)
		where T : Component;
}

















