using UnityEngine;
using System.Collections.Generic;

public class BodyParticleHelper 
{
	
	static Dictionary<ZigJointId,ZigJointId[]> sJointGroups = new Dictionary<ZigJointId, ZigJointId[]>
	{
		{ZigJointId.LeftElbow,new ZigJointId[]{ZigJointId.LeftElbow,ZigJointId.LeftShoulder,ZigJointId.Torso}},
		{ZigJointId.RightElbow,new ZigJointId[]{ZigJointId.RightElbow,ZigJointId.RightShoulder,ZigJointId.Torso}},
		{ZigJointId.LeftKnee,new ZigJointId[]{ZigJointId.LeftKnee,ZigJointId.LeftHip,ZigJointId.Waist}},
		{ZigJointId.RightKnee,new ZigJointId[]{ZigJointId.RightElbow,ZigJointId.RightHip,ZigJointId.Waist}},
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
	
	public void create_particles()
	{
		ManagerManager man = ManagerManager.Manager;
		NewGameManager ngm = man.mGameManager;
		BodyManager activeBody = man.mBodyManager;
		
		AdvancedGrading grade = null;
		
		foreach(var e in sJointGroups)
		{
			//float score = grade.joint_aggregate_score(e.Value);
			float score = 1;
			mParticles.emit_point(score,activeBody.mFlat.get_body_part_position(e.Key));
		}
		
		//mParticles.emit_rect(grade.CurrentScore,new Rect
	}
	
}
