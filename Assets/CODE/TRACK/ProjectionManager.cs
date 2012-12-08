using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ProjectionManager : FakeMonoBehaviour {
    public ProjectionManager(ManagerManager aManager) : base(aManager)
	{
	}
	
	public class Smoothing
	{
        public float snapInTolerance = 2.0f;
        public float snapOutTolerance = 4.0f;

        public bool snapped = false;
		public float target = 0;
		public float current = 0;

        public bool snap_change(float value, float snapValue, float interp)
        {
            bool r = false;
            float diff = Mathf.Abs(value - snapValue);
            if (diff > 180) diff -= 360;
            diff = Mathf.Abs(diff);
            if (snapped)
            {
                if (diff > snapOutTolerance)
                    snapped = false;
            }
            else
            {
                if (diff < snapInTolerance)
                {
                    snapped = true;
                    current = target = snapValue;
                    r = true;
                }
            }
            if (!snapped)
                target = value;
            else target = snapValue;

            set_current(interp);
            return r;
        }
        public void change(float value, float interp)
        {
            target = value;
            set_current(interp);
        }

        void set_current(float interp)
        {
            //current = interp * current + target * (1 - interp);
            
            if (current - target > 180)
                target += 360;
            else if (current - target < -180)
                target -= 360;
            current = interp * current + target * (1 - interp);
            if (target < -180)
                target += 360;
            else if (target > 180)
                target -= 360;
        }
	}
	//public Dictionary<GradingManager.WeightedZigJointPair,Smoothing> mImportant = new Dictionary<GradingManager.WeightedZigJointPair, Smoothing>(new GradingManager.WeightedZigJointPairComparer());

    public class Stupid
    {
        public ZigJointId otherEnd;
        public Smoothing smoothing = new Smoothing();
        public float weight = 1;
        public Stupid(ZigJointId other) { otherEnd = other; }
    }
    public Dictionary<ZigJointId, Stupid> mImportant = new Dictionary<ZigJointId, Stupid>();
	public override void Start () {
        /*mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftShoulder, ZigJointId.LeftElbow,1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftElbow, ZigJointId.LeftHand, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftHip, ZigJointId.LeftKnee, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.LeftKnee, ZigJointId.LeftAnkle, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightShoulder, ZigJointId.RightElbow, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightElbow, ZigJointId.RightHand, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightHip, ZigJointId.RightKnee, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.RightKnee, ZigJointId.RightAnkle, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.Neck, ZigJointId.Head, 1)] = new Smoothing();
        mImportant[new GradingManager.WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Neck, 1)] = new Smoothing();*/

        mImportant[ZigJointId.LeftShoulder] = new Stupid(ZigJointId.LeftElbow);
        mImportant[ZigJointId.LeftElbow] = new Stupid(ZigJointId.LeftHand);
        mImportant[ZigJointId.LeftHip] = new Stupid(ZigJointId.LeftKnee);
        mImportant[ZigJointId.LeftKnee] = new Stupid(ZigJointId.LeftAnkle);
        mImportant[ZigJointId.RightShoulder] = new Stupid(ZigJointId.RightElbow);
        mImportant[ZigJointId.RightElbow] = new Stupid(ZigJointId.RightHand);
        mImportant[ZigJointId.RightHip] = new Stupid(ZigJointId.RightKnee);
        mImportant[ZigJointId.RightKnee] = new Stupid(ZigJointId.RightAnkle);
        mImportant[ZigJointId.Neck] = new Stupid(ZigJointId.Head);
        mImportant[ZigJointId.Torso] = new Stupid(ZigJointId.Neck);
	}
    public Smoothing mWaist = new Smoothing();
	public Vector3 mNormal = Vector3.forward;
	public Vector3 mUp = Vector3.up;
    public float mSmoothing = 0.5f;
	
	public void compute_normal()
	{
		//TODO
	}
	public float get_smoothed_relative(ZigJointId A, ZigJointId B)
	{
		return mImportant[A].smoothing.current;
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
            foreach (KeyValuePair<ZigJointId,Stupid> e in mImportant)
            {
                if (e.Key != ZigJointId.None)
                {
                    ZigJointId parentJoint = BodyManager.get_parent(e.Key);
                    try
                    {
                        if (parentJoint == ZigJointId.None || (parentJoint == ZigJointId.Waist) || mImportant[parentJoint].smoothing.snapped == true)
                        {
                            if(e.Value.smoothing.snap_change(get_relative(mManager.mZigManager.Joints[e.Key], mManager.mZigManager.Joints[e.Value.otherEnd]), mManager.mTransparentBodyManager.mFlat.mTargetPose.find_element(e.Key).angle, mSmoothing))
                                ;// Debug.Log("SNAP " + e.Key);
                        }
                        else
                        {
                            e.Value.smoothing.change(get_relative(mManager.mZigManager.Joints[e.Key], mManager.mZigManager.Joints[e.Value.otherEnd]), mSmoothing);
                            
                        }
                    }
                    catch
                    {
                        //TODO wyh does this fail on first run?
                    }
                }
            }
            try
            {
                //mWaist.snap_change(get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]), mManager.mTransparentBodyManager.mTargetPose.find_element(ZigJointId.Waist).angle, mSmoothing);
                mWaist.change(get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]), mSmoothing);
                //mWaist.target = get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftHip], mManager.mZigManager.Joints[ZigJointId.RightHip]);
            }
            catch
            {
            }
        }
	}
}
