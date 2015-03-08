using UnityEngine;
//using System.Collections;


public class Movement : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		const float Scale = 50.0f;
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		var force =
			Vector3.right * x * Scale +
			Vector3.forward * y * Scale;

		var t = Time.deltaTime;
		var rb = GetComponent<Rigidbody>();

		rb.AddForce(force * t);
	}
}
