using UnityEngine;
using UnityEngine.UI;
//using System.Collections;


public class PlayerController : MonoBehaviour
{
	private int count;
	public Text scoreText;

	void Start()
	{
		count = 0;
		UpdateScore();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		const float Scale = 250.0f;
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		var force =
			Vector3.right * x * Scale +
			Vector3.forward * y * Scale;

		var t = Time.deltaTime;
		var rb = GetComponent<Rigidbody>();

		rb.AddForce(force * t);
	}

	void OnTriggerEnter(Collider rhs)
	{
		if (rhs.tag == "Pickup")
		{
			rhs.gameObject.SetActive(false);

			count++;
			UpdateScore();
		}
	}

	void UpdateScore()
	{
		scoreText.text = string.Format("Score: {0}", count);
	}
}
