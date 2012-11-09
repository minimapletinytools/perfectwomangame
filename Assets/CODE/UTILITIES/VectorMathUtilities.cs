using UnityEngine;
using System.Collections;

public class VectorMathUtilities {
	
	//finds closest point on segment AB
	public static Vector3 closest_point_on_segment(Vector3 A, Vector3 B, Vector3 p)
	{
		float lambda = Mathf.Clamp01(Vector3.Dot((p-A),(B-A))/(B-A).sqrMagnitude);
		return (1-lambda)*A + lambda*B;
	}
}
