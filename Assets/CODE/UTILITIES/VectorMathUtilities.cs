using UnityEngine;
using System.Collections;

public class VectorMathUtilities {
	
	public class MathHelper
	{
		public float ToRadians(float degrees){return degrees/180f*Mathf.PI;}
	}
	
	//finds closest point on segment AB
	public static Vector3 closest_point_on_segment(Vector3 A, Vector3 B, Vector3 p)
	{
		float lambda = Mathf.Clamp01(Vector3.Dot((p-A),(B-A))/(B-A).sqrMagnitude);
		return (1-lambda)*A + lambda*B;
	}
}
