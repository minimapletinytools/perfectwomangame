using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ProjectionManager : FakeMonoBehaviour {
    public ProjectionManager(ManagerManager aManager) : base(aManager)
	{
	}
	
	public class Smoothing
	{
		public float target = 0;
		public float current = 0;
	}
	public Dictionary<GradingManager.WeightedZigJointPair,Smoothing> mImportant = new Dictionary<GradingManager.WeightedZigJointPair, Smoothing>(new GradingManager.WeightedZigJointPairComparer());
	public override void Start () {
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftShoulder, ZigJointId.LeftElbow,1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftElbow, ZigJointId.LeftHand, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftHip, ZigJointId.LeftKnee, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftKnee, ZigJointId.LeftAnkle, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightShoulder, ZigJointId.RightElbow, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightElbow, ZigJointId.RightHand, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightHip, ZigJointId.RightKnee, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightKnee, ZigJointId.RightAnkle, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.Neck, ZigJointId.Head, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Neck, 1)] = new Smoothing();
		//mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Waist, 1)] = new Smoothing();
        //mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.None, ZigJointId.Torso, 1)] = new Smoothing();
	}
    public Smoothing mWaist = new Smoothing();
	public Vector3 mNormal = Vector3.forward;
	public Vector3 mUp = Vector3.up;
    public float mSmoothing = 0.6f;
	
	public void compute_normal()
	{
		//TODO
	}
	public float get_smoothed_relative(ZigInputJoint A, ZigInputJoint B)
	{
		return mImportant[new GradingManager.WeightedZigJointPair(A.Id,B.Id,1)].current;
	}
	public float get_relative(ZigInputJoint A, ZigInputJoint B)
	{
		if(A.Id == ZigJointId.None)
			return 0; //TODO
        return get_relative(A.Position, B.Position);
        /*TODO DELETE
		Vector3 right = Vector3.Cross(mUp,mNormal);
		Vector3 v = B.Position - A.Position;
		Vector3 projected = Vector3.Exclude(mNormal,v);
		float r = Vector3.Angle(right,v);
        if (Vector3.Dot(Vector3.Cross(right, projected), mNormal) < 0)
        {
            r *= -1;
        }
		return -r;*/
	}

    public float get_waist(ZigInputJoint waist, ZigInputJoint L, ZigInputJoint R)
    {
        return get_relative(waist.Position, L.Position * 0.5f + R.Position * 0.5f);
    }

    public float get_relative(Vector3 A, Vector3 B)
    {
        Vector3 right = Vector3.Cross(mUp, mNormal);
        Vector3 v = B - A;
        Vector3 projected = Vector3.Exclude(mNormal, v);
        float r = Vector3.Angle(right, v);
        if (Vector3.Dot(Vector3.Cross(right, projected), mNormal) < 0)
        {
            r *= -1;
        }
        return -r;
    }
	
	public override void Update () {
        if (mManager.mZigManager.has_user())
        {
            foreach (KeyValuePair<GradingManager.WeightedZigJointPair, Smoothing> e in mImportant)
            {
                if (e.Key.A != ZigJointId.None)
                {
                    try
                    {
                        e.Value.target = get_relative(mManager.mZigManager.Joints[e.Key.A], mManager.mZigManager.Joints[e.Key.B]);
                        e.Value.current = e.Value.current * mSmoothing + e.Value.target * (1-mSmoothing);//TODO smooth properly using Time.deltaTime
                    }
                    catch
                    {
                    }
                }
            }
            try
            {
                //mWaist.target = get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftHip], mManager.mZigManager.Joints[ZigJointId.RightHip]);
                mWaist.target = get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]);
                //go left or right
                //if(
                mWaist.current = mWaist.current * mSmoothing + mWaist.target * (1 - mSmoothing);
            }
            catch
            {
            }
        }
	}
}
