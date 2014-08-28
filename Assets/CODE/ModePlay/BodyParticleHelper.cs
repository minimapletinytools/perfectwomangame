using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class BodyParticleHelper 
{
	
	static Dictionary<ZgJointId,ZgJointId[]> sConnectedGroups = new Dictionary<ZgJointId, ZgJointId[]>
	{
		{ZgJointId.LeftShoulder,new ZgJointId[]{ZgJointId.LeftElbow}},
		{ZgJointId.LeftElbow,new ZgJointId[]{ZgJointId.LeftHand}},
		{ZgJointId.LeftHip,new ZgJointId[]{ZgJointId.LeftKnee}},
		{ZgJointId.LeftKnee,new ZgJointId[]{ZgJointId.LeftAnkle}},
		
		{ZgJointId.RightShoulder,new ZgJointId[]{ZgJointId.RightElbow}},
		{ZgJointId.RightElbow,new ZgJointId[]{ZgJointId.RightHand}},
		{ZgJointId.RightHip,new ZgJointId[]{ZgJointId.RightKnee}},
		{ZgJointId.RightKnee,new ZgJointId[]{ZgJointId.RightAnkle}},
		
		{ZgJointId.Neck,new ZgJointId[]{ZgJointId.Head}},
		{ZgJointId.Torso,new ZgJointId[]{ZgJointId.LeftShoulder,ZgJointId.RightShoulder}},
		{ZgJointId.Waist,new ZgJointId[]{ZgJointId.LeftHip,ZgJointId.RightHip}},
	};
	
	static Dictionary<ZgJointId,ZgJointId[]> sJointGroups = new Dictionary<ZgJointId, ZgJointId[]>
	{
		{ZgJointId.LeftHand,new ZgJointId[]{ZgJointId.LeftElbow,ZgJointId.LeftShoulder,ZgJointId.Torso}},
		{ZgJointId.RightHand,new ZgJointId[]{ZgJointId.RightElbow,ZgJointId.RightShoulder,ZgJointId.Torso}},
		{ZgJointId.LeftAnkle,new ZgJointId[]{ZgJointId.LeftKnee,ZgJointId.LeftHip,ZgJointId.Waist}},
		{ZgJointId.RightAnkle,new ZgJointId[]{ZgJointId.RightKnee,ZgJointId.RightHip,ZgJointId.Waist}},
		{ZgJointId.Head,new ZgJointId[]{ZgJointId.Neck,ZgJointId.Torso,ZgJointId.Waist}},
	};
	
	public SparkleStarFlashParticle mParticles;
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


		/*
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
		}*/
		
		

		if(!continuous)
		{

			bool fever = man.mGameManager.mModeNormalPlay.IsFever;

			float grade = ProGrading.grade_to_perfect(aGrade.CurrentGrade);

			int inc = Mathf.Clamp((int)(5*grade),0,4);

				
			if(inc < 1)
			{
				//TODO sad particles
			}
			if(inc >= 2 && inc < 3)
			{
				//mParticles.emit_ring("silver",5,activeBody.mFlat.get_body_part_position(ZigJointId.Torso),600, 1.5f);
				mParticles.emit_ring("silver",7,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),900, 1.5f);
			}
			if(inc >= 3  && inc <4)
			{
				mParticles.emit_ring("silver",5,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),700,1.5f);
				mParticles.emit_ring("gold",7,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),1000, 2f);
			}
			if(inc == 4)
			{
				if(grade > GameConstants.playSuperCutoff && fever)
				{
					mParticles.emit_ring("gold",20,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),2500,2f);
					//mParticles.emit_ring("gold",15,activeBody.mFlat.get_body_part_position(ZigJointId.Torso),1500,3f);
					mParticles.emit_ring("silver",12,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),1900, 3f);
					mParticles.emit_ring("gold",7,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),1200,4f);
					mParticles.emit_ring("silver",10,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),600,1.5f);
				}
				else
				{
					mParticles.emit_ring("gold",12,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),1900, 4f);
					mParticles.emit_ring("silver",7,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),1200,1.5f);
					mParticles.emit_ring("gold",10,activeBody.mFlat.get_body_part_position(ZgJointId.Torso),600,3f);
				}
			}


			float ag = (grade-0.4f)*(grade-0.4f);
			int count = (int)(100*ag);
			/*
			if(grade > 0.6f)
				mParticles.emit_point(count*1/3, activeBody.mFlat.get_body_part_position(ZigJointId.Torso),700);
			if(grade > 0.8f)
				mParticles.emit_point(count*2/3, activeBody.mFlat.get_body_part_position(ZigJointId.Torso),1000);
				*/
		}

		//TODO test this..
		if(continuous)
		{
			//if(man.mGameManager.mModeNormalPlay.IsFever)
			{
				//TODO this is the wrong camera lol...
				//FlatCameraManager cam = ManagerManager.Manager.mGameManager.mModeNormalPlay.mFlatCamera;
				//mParticles.emit_line("gold",1,cam.get_point(-1f,1.1f),cam.get_point(1f,1.1f),2);
			}
		}
		
		
		
		//mParticles.emit_rect(grade.CurrentScore,new Rect
	}
	
}
