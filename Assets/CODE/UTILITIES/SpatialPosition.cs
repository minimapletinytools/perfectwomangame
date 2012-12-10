using UnityEngine;
using System.Collections;
public class SpatialPosition : MonoBehaviour {
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
	static SpatialPosition interpolate_rotational(SpatialPosition A, SpatialPosition B, SpatialPosition axis, bool major, float t)
	{
		//TODO uggg finish this stupid function...
		SpatialPosition r = new SpatialPosition();
		Quaternion baseRotation = Quaternion.Slerp(A.r,B.r,t)*Quaternion.Inverse(A.r);
		Vector3 Av = A.p - axis.p;
		Vector3 Bv = B.p - axis.p;
		Vector3 up = axis.r*Vector3.forward;
		float theta = Vector3.Angle(Vector3.Exclude(up,Av),Vector3.Exclude(up,Bv)); //this is the non up component
		float phi = Vector3.Angle(Av,Vector3.Exclude(Vector3.Cross(Av,up).normalized,Bv)); //this is the angle in the up component
		bool right = Vector3.Dot(Vector3.Cross(Av,up),Bv) > 0; //this says Bv is to the right of Av relative to axis
		if(major) theta = Mathf.PI*2-theta;
		r.p = rotate_about(A.p,up,axis.p,t*theta*2*(System.Convert.ToInt32(right)-0.5f),0); //sure hope this is right lol
		Quaternion coarseRotation = Quaternion.FromToRotation(Av, r.p-axis.p); //may need inverse
		r.r = baseRotation*coarseRotation*A.r; //may be wrong order???
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
