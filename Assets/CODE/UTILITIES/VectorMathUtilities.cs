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
	
	
	
	private static Matrix OrthoX = Matrix.CreateRotationX(MathHelper.ToRadians(90));
    private static Matrix OrthoY = Matrix.CreateRotationY(MathHelper.ToRadians(90));

    public static void FindOrthonormals(Vector3 normal, out Vector3 orthonormal1, out Vector3 orthonormal2)
    {
        Vector3 w = Vector3.Transform(normal,OrthoX);
        float dot = Vector3.Dot(normal, w);
        if (Math.Abs(dot) > 0.6)
        {
            w = Vector3.Transform(normal,OrthoY);
        }
        w.Normalize();

        orthonormal1 = Vector3.Cross(normal, w);
        orthonormal1.Normalize();
        orthonormal2 = Vector3.Cross(normal, orthonormal1);
        orthonormal2.Normalize();
    }
	
	public static float FindQuaternionTwist(Quaternion q, Vector3 axis)
	{
	    axis.Normalize();
	
		
		//get the plane the axis is a normal of
		Vector3 orthonormal1, orthonormal2;
		ExMath.FindOrthonormals(axis, out orthonormal1, out orthonormal2);
		
		
		Vector3 transformed = Vector3.Transform(orthonormal1, q);
		
		
		//project transformed vector onto plane
		Vector3 flattened = transformed - (Vector3.Dot(transformed, axis) * axis);
		flattened.Normalize();
		
		
		//get angle between original vector and projected transform to get angle around normal
		float a = (float)Math.Acos((double)Vector3.Dot(orthonormal1, flattened));
		
		
		return a;
		
	
	}
}
