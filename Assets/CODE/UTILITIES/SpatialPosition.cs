using UnityEngine;
using System.Collections;
public class SpatialPosition {
	public Vector3 p = new Vector3();
	public Quaternion r = new Quaternion();
	public SpatialPosition(){}
	public SpatialPosition(Vector3 ap, Quaternion ar){p = ap; r = ar;}
	public SpatialPosition(Vector3 ap, Vector3 aForward){p = ap; r.SetFromToRotation(Vector3.forward,aForward);}
    public SpatialPosition(Transform at) { p = at.position; r = at.rotation; }
	public static SpatialPosition interpolate_linear(SpatialPosition A, SpatialPosition B, float t)
	{
		SpatialPosition r = new SpatialPosition();
		r.p = A.p*(1-t) + B.p*t;
		r.r = Quaternion.Slerp(A.r,B.r,t);
		return r;
	}
	static public Vector3 rotate_about(Vector3 aToRotate, Vector3 aUp, Vector3 aAbout, float theta, float phi)
    {
        Vector3 toRot = aToRotate - aAbout;
        aUp.Normalize();
        Vector3 aSide = Vector3.Cross(aUp, toRot).normalized;
        Vector3 composite = theta * aUp + phi * aSide;
        composite.Normalize();
        float rad = Mathf.Sqrt(theta*theta + phi*phi);
        toRot = Vector3.RotateTowards(toRot, composite, rad, 0);
        return toRot + aAbout;
    }
		
}
