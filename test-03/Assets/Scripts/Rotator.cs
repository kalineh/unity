using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	public float Speed = 1.0f;

	void Update()
	{
		float x = Speed;
		float y = Speed;
		float z = Speed;

		transform.Rotate(x, y, z);
	}
}
