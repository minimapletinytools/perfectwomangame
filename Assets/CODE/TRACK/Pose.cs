using UnityEngine;
using System.Collections.Generic;

//TODO move Pose and PoseElement into there
[System.Serializable]
public class PoseElement
{
    public ZigJointId joint;
    public float angle;
    public float weight = 1;
    public bool important = true; //this means we care about angle
    //other info if we want it...
}

[System.Serializable]
public class Pose
{
    public List<PoseElement> mElements = new List<PoseElement>();
    public PoseElement find_element(ZigJointId id)
    {
        foreach (PoseElement e in mElements)
            if (e.joint == id)
                return e;
        throw new UnityException("can't find ZigJointId " + id + " in Pose");
    }
	
	public static Pose interpolate(Pose A, Pose B, float lambda)
	{
		Pose r  = new Pose();
		foreach(PoseElement e in A.mElements)
		{
			PoseElement pe = new PoseElement();
			pe.joint = e.joint;
			pe.important = e.important;
			pe.weight = e.weight;
			pe.angle = (1-lambda) * e.angle +  (lambda)* B.find_element(e.joint).angle;
			r.mElements.Add(pe);
		}
		
		return r;
	}
}

public class PoseAnimation
{
	public List<Pose> poses = new List<Pose>();
	public Pose get_pose(int index){ return poses[index % poses.Count]; }
}