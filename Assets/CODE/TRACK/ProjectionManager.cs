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
	public override void Start () 
    {
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
	
	public float get_smoothed_relative(ZigJointId A, ZigJointId B)
	{
		return mImportant[A].smoothing.current;
	}
	public float get_relative(ZigInputJoint A, ZigInputJoint B)
	{
		float r = 0;
		if(A.Id == ZigJointId.None)
			return 0;
		if(!B.GoodPosition)
			r = -A.Rotation.flat_rotation() + 90;
		else
        	r = get_relative(A.Position, B.Position);


		/*
		if(B.Id == ZigJointId.Head)
		{
			r = Mathf.Clamp(r, 0, 180);
		}*/

		//openni fix to solve head being set to -90 angle problem
		if(B.Id == ZigJointId.Head)
			if(!B.GoodPosition)
				r = -90;

		/*
		if(B.Id == ZigJointId.Head)
			mManager.mDebugString = 
				(B.GoodPosition ? "true" : "false") + 
				" " + r.ToString() + 
				" " + (mImportant[ZigJointId.Torso].smoothing.current-90+mImportant[ZigJointId.Neck].smoothing.current);
				*/

        return r;
	}

    public float get_waist(ZigInputJoint waist, ZigInputJoint L, ZigInputJoint R)
    {

		float r = 0;
        if(false && !mManager.mZigManager.using_nite()) //TODO some problems with this lockngi... Should default to below if that happens
		//if(!mManager.mZigManager.using_nite())
            r = -waist.Rotation.flat_rotation() + 90;    
        else
            r = get_relative(waist.Position, L.Position * 0.5f + R.Position * 0.5f);

		return r;
    }

    public float get_relative(Vector3 A, Vector3 B)
    {
        Vector3 right = Vector3.Cross(mUp, mNormal);
        Vector3 v = B - A;
       
        Vector3 projected = Vector3.Exclude(mNormal, v);
        float r = Vector3.Angle(right, projected);
        if (Vector3.Dot(Vector3.Cross(right, projected), mNormal) < 0)
        {
            r *= -1;
        }
        return -r;
    }

    /*
    Dictionary<ZigJointId, GameObject> mDebugCharacter = new Dictionary<ZigJointId, GameObject>();
    public static ZigJointId[] mFullJoints = { ZigJointId.Neck, ZigJointId.LeftElbow, ZigJointId.LeftKnee, ZigJointId.LeftShoulder, ZigJointId.LeftHip, ZigJointId.RightElbow, ZigJointId.RightKnee, ZigJointId.RightShoulder, ZigJointId.RightHip, ZigJointId.Torso};
    public static ZigJointId[] mStubJoints = { ZigJointId.LeftHand, ZigJointId.RightHand, ZigJointId.LeftAnkle, ZigJointId.RightAnkle };
    public void create_debug_character()
    {
        GameObject parent = new GameObject("DEBUG_CHARACTER_PARENT");
        foreach (ZigJointId e in mFullJoints)
        {
            GameObject j = (GameObject)GameObject.Instantiate(mManager.mReferences.mDebugLimb);
            j.transform.parent = parent.transform;
            j.transform.localScale = new Vector3(30, 30, 30);
            mDebugCharacter[e] = j;
        }
        foreach (ZigJointId e in mStubJoints)
        {
            GameObject j = (GameObject)GameObject.Instantiate(mManager.mReferences.mDebugLimb);
            GameObject.Destroy(j.transform.FindChild("Cylinder"));
            j.transform.parent = parent.transform;
            j.transform.localScale = new Vector3(30, 30, 30);
            mDebugCharacter[e] = j;
        }
    }
    public void update_debug_character()
    {
        foreach (var e in mDebugCharacter)
        {
            e.Value.transform.position = mManager.mZigManager.Joints[e.Key].Position;
            e.Value.transform.rotation = mManager.mZigManager.Joints[e.Key].Rotation;
        }
    }*/

	public override void Update () {
        if (mManager.mZigManager.has_user())
        {
			Pose targetPose = mManager.mGameManager.CurrentTargetPose;
			Pose currentPose = mManager.mGameManager.CurrentPose; //this may have one frame of lag but oh well
			
            foreach (KeyValuePair<ZigJointId,Stupid> e in mImportant)
            {
                if (e.Key != ZigJointId.None && e.Key != ZigJointId.Waist)
                {
                    ZigJointId parentJoint = BodyManager.get_parent(e.Key);
                    try
                    {
                        /* note, if you do snapping, you need to turn it off for baby
                        if (parentJoint == ZigJointId.None || (parentJoint == ZigJointId.Waist) || mImportant[parentJoint].smoothing.snapped == true)
                        {
                            if(e.Value.smoothing.snap_change(get_relative(mManager.mZigManager.Joints[e.Key], mManager.mZigManager.Joints[e.Value.otherEnd]), mManager.mTransparentBodyManager.mFlat.mTargetPose.find_element(e.Key).angle, mSmoothing))
                                ;// Debug.Log("SNAP " + e.Key);
                        }
                        else*/
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
				
				float waistAngle = get_waist(mManager.mZigManager.Joints[ZigJointId.Torso], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]);
				//waist smoothing angle hack
				/*
				if(!mManager.mZigManager.using_nite() && targetPose != null)
				{
					float interp = Mathf.Clamp01(
						(ProGrading.grade_joint(currentPose,targetPose,ZigJointId.LeftHip) + 
						ProGrading.grade_joint(currentPose,targetPose,ZigJointId.RightHip))/100f);
					waistAngle = targetPose.find_element(ZigJointId.Waist).angle * (1-interp) + waistAngle * interp;
				}*/
				mWaist.change(waistAngle,mSmoothing);
				
                //mWaist.snap_change(get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]), mManager.mTransparentBodyManager.mTargetPose.find_element(ZigJointId.Waist).angle, mSmoothing);
                //mWaist.change(get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftKnee], mManager.mZigManager.Joints[ZigJointId.RightKnee]), mSmoothing);
                //mWaist.target = get_waist(mManager.mZigManager.Joints[ZigJointId.Waist], mManager.mZigManager.Joints[ZigJointId.LeftHip], mManager.mZigManager.Joints[ZigJointId.RightHip]);
            }
            catch
            {
            }
        }
	}
}
