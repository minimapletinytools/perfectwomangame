using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//this class contains info on per-joint grade
public class AdvancedGrading
{
	Dictionary<ZigJointId,float> mLastScore = null;
	Dictionary<ZigJointId,float> mCurrentScore = null;
	
	public AdvancedGrading()
	{
		
	}
	
	public void update()
	{
		
		//TODO make global??
		float lambda = 0.9f;
		//TODO
		Dictionary<ZigJointId,float> newScore = null;//ProGrading.advanced_grade_pose(CurrentPose, CurrentTargetPose);
		mCurrentScore = new Dictionary<ZigJointId, float>(newScore);
		foreach(KeyValuePair<ZigJointId,float> e in newScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + newScore[e.Key]*(1-lambda);
		}
		mLastScore = newScore;
	}
	
	public float joint_aggregate_score(ZigJointId[] aJoints)
	{
		float r = 0;
		foreach(ZigJointId e in aJoints)
		{
			r += mCurrentScore[e];	
		}
		return r*mCurrentScore.Count/aJoints.Length;
	}
	
	public float CurrentScore
	{
		get{
			return mCurrentScore.Aggregate((e,f) => new KeyValuePair<ZigJointId,float>(e.Key,e.Value + f.Value)).Value;
		}
	}
	
		
}
