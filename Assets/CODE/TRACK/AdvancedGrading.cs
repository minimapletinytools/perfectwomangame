using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//this class contains info on per-joint grade
public class AdvancedGrading
{
	
	const float GRADE_INTERP = 0.9f;
	Dictionary<ZigJointId,float> mLastScore = null;
	Dictionary<ZigJointId,float> mCurrentScore = null;
	
	public AdvancedGrading()
	{
		
	}
	
	public void update(Pose aCurrent, Pose aTarget)
	{
		
		//TODO make global??
		float lambda = GRADE_INTERP;
		Dictionary<ZigJointId,float> newScore = ProGrading.advanced_grade_pose(aCurrent, aTarget);
		if(mCurrentScore == null)
			mCurrentScore = newScore;
		mLastScore = mCurrentScore;
		mCurrentScore = new Dictionary<ZigJointId, float>(newScore);
		foreach(KeyValuePair<ZigJointId,float> e in newScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + newScore[e.Key]*(1-lambda);
		}
	}
	
	//only for testing purposes
	public void fake_update(float aGrade)
	{
		float lambda = GRADE_INTERP;
		mLastScore = mCurrentScore;
		mCurrentScore = new Dictionary<ZigJointId, float>();
		foreach(KeyValuePair<ZigJointId,float> e in mLastScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + aGrade*(1-lambda);
		}
	}
	
	public float joint_score(ZigJointId aJoint)
	{
		return mCurrentScore[aJoint];
	}
	
	public float joint_aggregate_score(ZigJointId[] aJoints)
	{
		float r = 0;
		foreach(ZigJointId e in aJoints)
		{
			r += mCurrentScore[e]*mCurrentScore[e];	
		}
		return Mathf.Sqrt(r)/aJoints.Length;
	}
	
	public float CurrentGrade
	{
		get{
			float r = 0;
			foreach(var e in mCurrentScore)
			{
				r += e.Value*e.Value;
			}
			return Mathf.Sqrt(r)/mCurrentScore.Count;
		}
	}
	
		
}
