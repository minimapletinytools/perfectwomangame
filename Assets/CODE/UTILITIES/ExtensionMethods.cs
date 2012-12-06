using UnityEngine;
using System.Collections;

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


    //Quaternion stuff
    public static float flat_rotation(this Quaternion aQuat)
    {
        return aQuat.eulerAngles.z;
    }
}
