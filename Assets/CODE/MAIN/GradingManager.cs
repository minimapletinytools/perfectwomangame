using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GradingManager : FakeMonoBehaviour {
    
	public GradingManager(ManagerManager aManager) : base(aManager) {}

    public class WeightedZigJointPair
    {
        public ZigJointId A { get; set; }
        public ZigJointId B { get; set; }
        public float weight { get; set; }
        public WeightedZigJointPair(ZigJointId A, ZigJointId B, float weight) { this.A = A; this.B = B; this.weight = weight; }
    }
	public class WeightedZigJointPairComparer  : IEqualityComparer<WeightedZigJointPair> 
	{
		public bool Equals(WeightedZigJointPair x, WeightedZigJointPair y)
	    {
	        return x.A == y.A && x.B == y.B;
	    }
		public int GetHashCode(WeightedZigJointPair pair)
	    {
			return ((int)pair.A)*100+((int)pair.B);
	    }
	}
    public class Pose
    {
        public Dictionary<ZigJointId, ZigInputJoint> mPose = new Dictionary<ZigJointId, ZigInputJoint>();

    }

    List<WeightedZigJointPair> mImportant = new List<WeightedZigJointPair>();
    List<Pose> mPoses = new List<Pose>();
	public override void Start () {
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftShoulder, ZigJointId.LeftElbow, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftElbow, ZigJointId.LeftHand, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftHip, ZigJointId.LeftKnee, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftKnee, ZigJointId.LeftAnkle, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Neck, ZigJointId.Head, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Neck, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Waist, 1));
		mImportant.Add(new WeightedZigJointPair(ZigJointId.None, ZigJointId.Torso, 1));
        
        Pose p = new Pose();
	}

    public void record_pose()
    {
        Pose p = new Pose();
        p.mPose = new Dictionary<ZigJointId, ZigInputJoint>(mManager.mZigManager.Joints);
        mPoses.Add(p);
    }
    public float grade_pose(Pose aPose)
    {
        float weightsum = 0;
        float gradesum = 0;
        foreach (WeightedZigJointPair e in mImportant)
        {
			float target = mManager.mProjectionManager.get_relative(aPose.mPose[e.A],aPose.mPose[e.B]);
			float actual = mManager.mProjectionManager.get_relative(mManager.mZigManager.Joints[e.A],mManager.mZigManager.Joints[e.B]);
			float diff = target-actual;
			if(diff > Mathf.PI) diff -= Mathf.PI;
            gradesum += diff*diff*e.weight;
            weightsum += e.weight;
        }
		return Mathf.Sqrt(gradesum)/weightsum;
    }
    public override void Update()
    {
	    
	}
}
