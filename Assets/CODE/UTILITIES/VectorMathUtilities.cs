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

public class PolygonalPath
{
	
	Vector3[] mPoints = null;
	float[] mLengths;
	
	public PolygonalPath(Vector3[] aPoints)
	{
		mPoints = new Vector3[aPoints.Length];
		
		aPoints.CopyTo(mPoints,0);
		
		if(mPoints.Length > 1)
		{
			mLengths = new float[aPoints.Length-1];
			for(int i = 0; i < mPoints.Length-1; i++)
			{
				mLengths[i] = (mPoints[i+1]-mPoints[i]).magnitude;
				if(i > 0)
					mLengths[i] += mLengths[i-1];
			}
			
		}
	}
	
	//aT in [0,1]
	public Vector3 evaluate(float aT)
	{
		if(mPoints == null)
			return Vector3.zero;
		if(mPoints.Length == 1)
			return mPoints[0];
		float val = aT * mLengths[mLengths.Length-1];
		for(int i = 0; i < mPoints.Length-1; i++)
		{
			if(val < mLengths[i]) //that means we are in segment [i,i+1]
			{
				float prev = (i == 0 ? 0 : mLengths[i-1]);
				float lambda = (mLengths[i] - prev) == 0 ? 0 : (aT - prev) / (mLengths[i] - prev);
				return mPoints[i]*(1-lambda) + mPoints[i+1]*lambda;
			}
		}
		
		return mPoints[mPoints.Length-1];
	}
	
	public float PathLength {get{return mLengths[mLengths.Length-1];}}
	
}
