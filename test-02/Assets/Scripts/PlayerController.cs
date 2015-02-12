using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	[Tooltip("Movement speed")]
	[Range(5.0f, 25.0f)]
	public float Speed = 5.0f;

	// called before rendering a frame, where most of game code will go
	void Update()
	{

	}

	// called just before physics update
	void FixedUpdate()
	{
		float moveh = Input.GetAxis("Horizontal");
		float movev = Input.GetAxis("Vertical");

		var force = 
			Vector3.forward * movev +
			Vector3.right * moveh;

		rigidbody.AddForce(force * this.Speed);

	}
}
