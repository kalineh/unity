using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Seed : MonoBehaviour
{
	private int generation;

	void Start ()
	{
		StartCoroutine("tGrowth");
	}
	
	void Update ()
	{
	}

	void Grow()
	{
		var hinge = GetComponent<HingeJoint>();
		var body = GetComponent<Rigidbody>();
		var collider = GetComponent<SphereCollider>();
		
		var child = Instantiate(this);

		child.generation = this.generation + 1;

		var scale = 1.0f / child.generation;
		var scale3 = new Vector3(scale, scale, scale);

		var ofs = Random.onUnitSphere * collider.radius;
		var force = Random.onUnitSphere * 2.5f;

		var child_body = child.GetComponent<Rigidbody>();
		var child_collider = child.GetComponent<SphereCollider>();
		var child_mesh = child.GetComponent<MeshRenderer>();

		child_body.mass = scale;
		child_body.MovePosition(ofs);
		child_body.AddForce(force);
		//child_collider.radius = scale;
		child_mesh.transform.localScale = scale3;

		hinge.connectedBody = child.GetComponent<Rigidbody>();
	}

	IEnumerator tGrowth()
	{
		float interval = generation * generation * 0.25f;
		float increment = 0.1f;

		while (true)
		{
			yield return new WaitForSeconds(interval);
			interval += increment;
			Grow();
		}
	}
}
