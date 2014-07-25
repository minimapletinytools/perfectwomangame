using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//this class contains info on per-joint grade
public class AdvancedGrading
{
	
	const float GRADE_INTERP = 0f; //0.9f;
	Dictionary<ZgJointId,float> mLastScore = null;
	Dictionary<ZgJointId,float> mCurrentScore = null;
	
	public AdvancedGrading()
	{
		
	}
	
	public void update(Pose aCurrent, Pose aTarget)
	{
		
		//TODO make global??
		float lambda = GRADE_INTERP;
		Dictionary<ZgJointId,float> newScore = ProGrading.advanced_grade_pose(aCurrent, aTarget);
		if(mCurrentScore == null)
			mCurrentScore = newScore;
		mLastScore = mCurrentScore;
		mCurrentScore = new Dictionary<ZgJointId, float>(newScore);
		foreach(KeyValuePair<ZgJointId,float> e in newScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + newScore[e.Key]*(1-lambda);
		}
	}
	
	//only for testing purposes
	public void fake_update(float aGrade)
	{
		float lambda = GRADE_INTERP;
		mLastScore = mCurrentScore;
		mCurrentScore = new Dictionary<ZgJointId, float>();
		foreach(KeyValuePair<ZgJointId,float> e in mLastScore)
		{
			mCurrentScore[e.Key] = mLastScore[e.Key]*lambda + aGrade*(1-lambda);
		}
	}
	
	public float joint_score(ZgJointId aJoint)
	{
		return mCurrentScore[aJoint];
	}
	
	public float joint_aggregate_score(ZgJointId[] aJoints)
	{
		float r = 0;
		foreach(ZgJointId e in aJoints)
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
