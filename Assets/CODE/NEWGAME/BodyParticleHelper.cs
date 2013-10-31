using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class BodyParticleHelper 
{
	
	static Dictionary<ZigJointId,ZigJointId[]> sJointGroups = new Dictionary<ZigJointId, ZigJointId[]>
	{
		{ZigJointId.LeftElbow,new ZigJointId[]{ZigJointId.LeftElbow,ZigJointId.LeftShoulder,ZigJointId.Torso}},
		{ZigJointId.RightElbow,new ZigJointId[]{ZigJointId.RightElbow,ZigJointId.RightShoulder,ZigJointId.Torso}},
		{ZigJointId.LeftKnee,new ZigJointId[]{ZigJointId.LeftKnee,ZigJointId.LeftHip,ZigJointId.Waist}},
		{ZigJointId.RightKnee,new ZigJointId[]{ZigJointId.RightKnee,ZigJointId.RightHip,ZigJointId.Waist}},
	};
	
	SparkleStarFlashParticle mParticles;
	public BodyParticleHelper()
	{
		mParticles = new SparkleStarFlashParticle();
	}
	
	
	
	
	public void update(float aDelta)
	{
		mParticles.update(aDelta);
	}
	
	public void create_particles(AdvancedGrading aGrade)
	{
		ManagerManager man = ManagerManager.Manager;
		NewGameManager ngm = man.mGameManager;
		BodyManager activeBody = man.mBodyManager;
		
		Dictionary<ZigJointId,float> output = new Dictionary<ZigJointId, float>();
		foreach(var e in sJointGroups)
		{
			float score = ProGrading.grade_to_perfect(aGrade.joint_aggregate_score(e.Value));
			output[e.Key] = score;
			mParticles.emit_point(score,activeBody.mFlat.get_body_part_position(e.Key));
		}
		
		string s = "";
		foreach(var e in output)
			s += e.Value + " ";
		Debug.Log(s);
		
		//mParticles.emit_rect(grade.CurrentScore,new Rect
	}
	
}
