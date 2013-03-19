using UnityEngine;
using System.Collections.Generic;

//TODO move Pose and PoseElement into there
	
public class PoseAnimation
{
	public List<ProGrading.Pose> poses = new List<ProGrading.Pose>();
	public ProGrading.Pose get_pose(int index){ return poses[index % poses.Count]; }
}