using UnityEngine;
using System.Collections;

public class FixedJointChanger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			var joint = GetComponent<HingeJoint>();
			var limits = joint.limits;
			limits.min = limits.max = limits.min + 10;
			joint.limits = limits;

		}
	}
}
