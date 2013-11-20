using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class BodyParticleHelper 
{
	
	static Dictionary<ZigJointId,ZigJointId[]> sJointGroups = new Dictionary<ZigJointId, ZigJointId[]>
	{
		{ZigJointId.LeftHand,new ZigJointId[]{ZigJointId.LeftElbow,ZigJointId.LeftShoulder,ZigJointId.Torso}},
		{ZigJointId.RightHand,new ZigJointId[]{ZigJointId.RightElbow,ZigJointId.RightShoulder,ZigJointId.Torso}},
		{ZigJointId.LeftAnkle,new ZigJointId[]{ZigJointId.LeftKnee,ZigJointId.LeftHip,ZigJointId.Waist}},
		{ZigJointId.RightAnkle,new ZigJointId[]{ZigJointId.RightKnee,ZigJointId.RightHip,ZigJointId.Waist}},
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
	
	
	public void create_particles(AdvancedGrading aGrade, bool continuous = false)
	{
		ManagerManager man = ManagerManager.Manager;
		NewGameManager ngm = man.mGameManager;
		BodyManager activeBody = man.mBodyManager;
		
		Dictionary<ZigJointId,float> output = new Dictionary<ZigJointId, float>();
		foreach(var e in sJointGroups)
		{
			float score = ProGrading.grade_to_perfect(aGrade.joint_aggregate_score(e.Value));
			output[e.Key] = score;
			//if(!continuous)
				//mParticles.emit_point(score,activeBody.mFlat.get_body_part_position(e.Key),200);
			//else
				mParticles.emit_continuous(score,activeBody.mFlat.get_body_part_position(e.Key));
			
		}
		
		if(!continuous)
		{
			mParticles.emit_point(ProGrading.grade_to_perfect(aGrade.CurrentGrade), activeBody.mFlat.get_body_part_position(ZigJointId.Torso),1000);
		}
		
		string s = "";
		foreach(var e in output)
			s += e.Value + " ";
		//Debug.Log(s);
		
		//mParticles.emit_rect(grade.CurrentScore,new Rect
	}
	
}
