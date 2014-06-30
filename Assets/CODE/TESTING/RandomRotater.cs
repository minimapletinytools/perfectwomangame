using UnityEngine;
using System.Collections;

public class RandomRotater : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

    }
}
