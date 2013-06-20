using UnityEngine;
using System.Collections.Generic;

public static class ExtensionMethods  {
	public static Vector3 component_multiply(this Vector3 aVec, Vector3 scaleVec)
    {
        return new Vector3(aVec.x*scaleVec.x,aVec.y*scaleVec.y,aVec.z*scaleVec.z);
    }
	public static Vector3 component_inverse(this Vector3 aVec)
    {
        return new Vector3(1/aVec.x, 1/aVec.y, 1/aVec.z);
    }
	public static Vector3 global_scale(this Transform aTrans)
	{
		if(aTrans.parent == null)
			return aTrans.localScale;
		return aTrans.localScale.component_multiply(global_scale(aTrans.parent));
	}

	
	//rect stuf
	public static Rect union(this Rect A, Rect B)
	{
		Rect r = A;
		r.xMin = Mathf.Min(A.xMin,B.xMin);
		r.yMin = Mathf.Min(A.yMin,B.yMin);
		r.xMax = Mathf.Max(A.xMax,B.xMax);
		r.yMax = Mathf.Max(A.yMax,B.yMax);
		return r;
	}

    //Quaternion stuff
    public static float flat_rotation(this Quaternion aQuat)
    {
        return aQuat.eulerAngles.z;
    }
	
	//textasset
	public static Pose to_pose(this TextAsset aAsset)
	{
		return ProGrading.read_pose(aAsset);
	}
	
	//system
	
	
	//lolo repeat
	public static void Shuffle<T>(this IList<T> list)  
	{  
	    System.Random rng = new System.Random();  
	    int n = list.Count;  
	    while (n > 1) {  
	        n--;  
	        int k = rng.Next(n + 1);  
	        T value = list[k];  
	        list[k] = list[n];  
	        list[n] = value;  
	    }  
	}
	
	public static Rect expand(this Rect r, float exp)
	{
		r.x -= 	exp;
		r.y -= exp;
		r.width += 2*exp;
		r.height += 2*exp;
		return r;
	}
	
	//----------
	//bounds nonsense
	//----------
	public static Bounds to_bounds(this Vector3 p)
	{
		Bounds r = new Bounds();
		r.center = p;
		r.extents = new Vector3(0,0,0);
		return r;
	}
	
	public static Bounds union(this Bounds A, Vector3 p)
	{
		Bounds r = new Bounds(A.center,A.size);
		r.Encapsulate(p);	
		return r;
	}
	
	public static Bounds union(this Bounds A, Bounds o)
	{
		Bounds r = new Bounds(A.center,A.size);
		r.Encapsulate(o);
		return r;
	}
	
	
	public static Bounds bounds_from_points(IEnumerable<Vector3> aPoints)
	{
		Bounds? r = null;
		foreach(Vector3 e in aPoints)
		{
			if (r == null)
				r = e.to_bounds();
			else
				r = r.Value.union(e);
		}
		return r.Value;
	}
}
