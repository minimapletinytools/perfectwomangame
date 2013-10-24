using UnityEngine;
using System.Collections;

public class VectorMathUtilities {
	
	//TODO does not belong here
	public static class MathHelper
	{
		public static float ToRadians(float degrees){return degrees/180f*Mathf.PI;}
		public static float interpolate_degrees(float A, float B, float lambda)
		{
			while (Mathf.Abs(A-B) > 180)
			{
				if(A > B) 
					A -= 360;
				else B -= 360;
			}
			float r = (1-lambda) * A +  (lambda)* B;
			return r;
		}
	}
	
	//finds closest point on segment AB
	public static Vector3 closest_point_on_segment(Vector3 A, Vector3 B, Vector3 p)
	{
		float lambda = Mathf.Clamp01(Vector3.Dot((p-A),(B-A))/(B-A).sqrMagnitude);
		return (1-lambda)*A + lambda*B;
	}
	
	
}
