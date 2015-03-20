using UnityEngine;
using UnityEditor;
using System.Collections;
using Random = UnityEngine.Random;

[CustomEditor(typeof(Seed))]
public class SeedDebugMembers
	: DebugMembers<Seed>
{
}

public class Seed
	: MonoBehaviour
{
	private int generation = 1;
	private Seed parent;

	void Start ()
	{
	}
	
	void Update ()
	{
	}

	void Grow()
	{
		var body = GetComponent<Rigidbody>();
		var collider = GetComponent<SphereCollider>();

		if (generation > 5)
		{
			return;
		}

		var child = Instantiate(this);

		child.generation = this.generation + 1;
		child.parent = this;

		child.gameObject.AddComponent<SpringJoint>();

		var scale = 1.0f / child.generation;
		var scale3 = new Vector3(scale, scale, scale);

		var dir = Random.onUnitSphere;
		var edge = dir * collider.radius;
		var ofs = edge + dir * scale;
		var force = dir * 5.0f;

		var child_body = child.GetComponent<Rigidbody>();
		var child_collider = child.GetComponent<SphereCollider>();
		var child_mesh = child.GetComponent<MeshRenderer>();
		var child_spring = child.GetComponent<SpringJoint>();

		child_body.isKinematic = false;
		child_body.mass = scale;
		child_mesh.transform.localScale = scale3;

		child_body.MovePosition(ofs);
		child_body.AddForce(force);

		child_spring.connectedBody = body;
		child_spring.anchor = edge;
	}

	IEnumerator tGrowth()
	{
		float interval = generation * generation * 0.15f;
		float increment = 0.25f;

		while (true)
		{
			yield return new WaitForSeconds(interval);
			interval += increment;
			Grow();
		}
	}
}
