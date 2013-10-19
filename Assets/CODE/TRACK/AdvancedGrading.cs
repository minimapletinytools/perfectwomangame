using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//this class contains info on per-joint grade
public class AdvancedGrading
{
	float mLastWeightSum = 1;
	Dictionary<ZigJointId,float> mLastScore = null;
	float mCurrentWeightSum = 1;
	Dictionary<ZigJointId,float> mCurrentScore = null;
	
	public AdvancedGrading()
	{
		
	}
	
	public void update()
	{
		
		//TODO make global??
		float lambda = 0.9f;
		//TODO
		var newScore = ProGrading.advanced_grade_pose(CurrentPose, CurrentTargetPose);
		mCurrentScore = new Dictionary<ZigJointId, float>(newScore);
		foreach(KeyValuePair<ZigJointId,float> e in newScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + newScore[e.Key]*(1-lambda);
		}
		mLastScore = newScore;
	}
	
	public float CurrentScore
	{
		get{
			return mCurrentScore.Aggregate((e,f) => new KeyValuePair<ZigJointId,float>(e.Key,e.Value + f.Value)).Value;
		}
	}
		
}
