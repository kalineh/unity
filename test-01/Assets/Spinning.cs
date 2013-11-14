using UnityEngine;
using System.Collections;

public class Spinning : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        var t = Time.time;
        var s = Mathf.Cos(t * 0.01f) * 0.01f;
        var transform = GetComponent<Transform>();

        transform.Translate(s, 0.0f, 0.0f);
	}
}
