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


    [System.Serializable]
    public class SerializableZigInputJoint
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public ZigJointId mId;
        public SerializableZigInputJoint(ZigJointId id){mId = id;}
        public SerializableZigInputJoint() { }
    }


    public class Pose
    {
        public Dictionary<ZigJointId, ZigInputJoint> mPose = new Dictionary<ZigJointId, ZigInputJoint>();
        public List<KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint>> serializableList = new List<KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint>>();

        public void construct_pose_from_serialized()
        {
            foreach (KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint> e in serializableList)
            {
                mPose[e.Key] = new ZigInputJoint(e.Value.mId,e.Value.Position,e.Value.Rotation,false);   
            }
        }

        public void construct_serialized_from_pose()
        {
            foreach(KeyValuePair<ZigJointId, ZigInputJoint> e in mPose)
            {
                GradingManager.SerializableZigInputJoint joint = new GradingManager.SerializableZigInputJoint(e.Value.Id);
                joint.Position = e.Value.Position;
                joint.Rotation = e.Value.Rotation;
                serializableList.Add(new KeyValuePair<ZigJointId, SerializableZigInputJoint>(e.Key, joint));
            }
        }
    }

    List<WeightedZigJointPair> mImportant = new List<WeightedZigJointPair>();
    List<Pose> mPoses = new List<Pose>();
	public override void Start () {
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftShoulder, ZigJointId.LeftElbow, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftElbow, ZigJointId.LeftHand, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftHip, ZigJointId.LeftKnee, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.LeftKnee, ZigJointId.LeftAnkle, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.RightShoulder, ZigJointId.RightElbow, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.RightElbow, ZigJointId.RightHand, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.RightHip, ZigJointId.RightKnee, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.RightKnee, ZigJointId.RightAnkle, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Neck, ZigJointId.Head, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Neck, 1));
        mImportant.Add(new WeightedZigJointPair(ZigJointId.Torso, ZigJointId.Waist, 1));
		mImportant.Add(new WeightedZigJointPair(ZigJointId.None, ZigJointId.Torso, 1));
        
        Pose p = new Pose();
	}

    public Pose read_pose(TextAsset aText)
    {
        Pose p = new Pose();
        System.IO.MemoryStream stream = new System.IO.MemoryStream(aText.bytes);
        //System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint>>));
        p.serializableList = (List<KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint>>)xs.Deserialize(stream);
        p.construct_pose_from_serialized();
        return p;
    }
    public void write_pose_to_file(Pose p, string aFile)
    {
        System.IO.Stream stream = System.IO.File.Open(aFile, System.IO.FileMode.Create);
        //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<KeyValuePair<ZigJointId, GradingManager.SerializableZigInputJoint>>));
        p.construct_serialized_from_pose();
        //bFormatter.Serialize(stream, p.serializablePose);
        xs.Serialize(stream, p.serializableList);
        stream.Close();
        Debug.Log("write file " + aFile);
    }
    public string print_pose()
    {   
        record_pose();
        return print_pose(mPoses[0]);
    }
    public string print_pose(Pose aPose)
    {
        string s = "";
        foreach (WeightedZigJointPair e in mImportant)
        {
            if (e.A != ZigJointId.None)
            {
                Debug.Log(e.A.ToString() + e.B.ToString());
                //this prints the position from kinect
                //float actual = mManager.mProjectionManager.get_relative(mManager.mZigManager.Joints[e.A], mManager.mZigManager.Joints[e.B]);

                float actual = mManager.mProjectionManager.get_relative(aPose.mPose[e.A], aPose.mPose[e.B]);
                s += e.A.ToString() + " " + e.B.ToString() + " " + actual.ToString() + "\n";
            }
        }
        return s;
    }
    public Pose record_pose()
    {
        Pose p = new Pose();
        p.mPose = new Dictionary<ZigJointId, ZigInputJoint>(mManager.mZigManager.Joints);
        return p; 
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
