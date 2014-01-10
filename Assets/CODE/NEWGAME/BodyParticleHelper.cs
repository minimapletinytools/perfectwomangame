using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class BodyParticleHelper 
{
	
	static Dictionary<ZigJointId,ZigJointId[]> sConnectedGroups = new Dictionary<ZigJointId, ZigJointId[]>
	{
		{ZigJointId.LeftShoulder,new ZigJointId[]{ZigJointId.LeftElbow}},
		{ZigJointId.LeftElbow,new ZigJointId[]{ZigJointId.LeftHand}},
		{ZigJointId.LeftHip,new ZigJointId[]{ZigJointId.LeftKnee}},
		{ZigJointId.LeftKnee,new ZigJointId[]{ZigJointId.LeftAnkle}},
		
		{ZigJointId.RightShoulder,new ZigJointId[]{ZigJointId.RightElbow}},
		{ZigJointId.RightElbow,new ZigJointId[]{ZigJointId.RightHand}},
		{ZigJointId.RightHip,new ZigJointId[]{ZigJointId.RightKnee}},
		{ZigJointId.RightKnee,new ZigJointId[]{ZigJointId.RightAnkle}},
		
		{ZigJointId.Neck,new ZigJointId[]{ZigJointId.Head}},
		{ZigJointId.Torso,new ZigJointId[]{ZigJointId.LeftShoulder,ZigJointId.RightShoulder}},
		{ZigJointId.Waist,new ZigJointId[]{ZigJointId.LeftHip,ZigJointId.RightHip}},
	};
	
	static Dictionary<ZigJointId,ZigJointId[]> sJointGroups = new Dictionary<ZigJointId, ZigJointId[]>
	{
		{ZigJointId.LeftHand,new ZigJointId[]{ZigJointId.LeftElbow,ZigJointId.LeftShoulder,ZigJointId.Torso}},
		{ZigJointId.RightHand,new ZigJointId[]{ZigJointId.RightElbow,ZigJointId.RightShoulder,ZigJointId.Torso}},
		{ZigJointId.LeftAnkle,new ZigJointId[]{ZigJointId.LeftKnee,ZigJointId.LeftHip,ZigJointId.Waist}},
		{ZigJointId.RightAnkle,new ZigJointId[]{ZigJointId.RightKnee,ZigJointId.RightHip,ZigJointId.Waist}},
		{ZigJointId.Head,new ZigJointId[]{ZigJointId.Neck,ZigJointId.Torso,ZigJointId.Waist}},
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
		BodyManager activeBody = man.mBodyManager;
	
		/*foreach(var e in sJointGroups)
		{
			float score = ProGrading.grade_to_perfect(aGrade.joint_aggregate_score(e.Value));
			//if(!continuous)
				//mParticles.emit_point(score,activeBody.mFlat.get_body_part_position(e.Key),200);
			//else
				mParticles.emit_continuous(score,activeBody.mFlat.get_body_part_position(e.Key));
		}*/
		
		foreach(var e in sConnectedGroups)
		{
			float score = ProGrading.grade_to_perfect(aGrade.joint_score(e.Key));
			Vector3[] path = new Vector3[e.Value.Length+1];
			path[0] = activeBody.mFlat.get_body_part_position(e.Value[0]);
			path[1] = activeBody.mFlat.get_body_part_position(e.Key);
			if(e.Value.Length == 2)
				path[2] = activeBody.mFlat.get_body_part_position(e.Value[1]);
			PolygonalPath pPath = new PolygonalPath(path);
			
			for(int i = 0; i < pPath.PathLength/20f; i++)
			{
				mParticles.emit_continuous(score,pPath.evaluate((float)i*20/pPath.PathLength));
			}
			
			if(pPath.PathLength == 0)
				mParticles.emit_continuous(score,pPath.evaluate(0));
		}
		
		
		//TODO double it!
		if(!continuous)
		{
			float grade = ProGrading.grade_to_perfect(aGrade.CurrentGrade);
			float ag = (grade-0.4f)*(grade-0.4f);
			int count = (int)(100*ag);
				
			if(grade > 0.6f)
				mParticles.emit_point(count*1/3, activeBody.mFlat.get_body_part_position(ZigJointId.Torso),700);
			if(grade > 0.8f)
				mParticles.emit_point(count*2/3, activeBody.mFlat.get_body_part_position(ZigJointId.Torso),1000);
		}
		
		
		
		//mParticles.emit_rect(grade.CurrentScore,new Rect
	}
	
}
