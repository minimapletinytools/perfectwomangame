using UnityEngine;
using System.Collections.Generic;

public class ProGrading {
    public static List<KeyValuePair<ZigJointId, ZigJointId>> sPairs;
    static ProGrading()
    {
        sPairs = new List<KeyValuePair<ZigJointId, ZigJointId>>();
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftShoulder, ZigJointId.LeftElbow));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftElbow, ZigJointId.LeftHand));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftHip, ZigJointId.LeftKnee));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftKnee, ZigJointId.LeftAnkle));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightShoulder, ZigJointId.RightElbow));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightElbow, ZigJointId.RightHand));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightHip, ZigJointId.RightKnee));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightKnee, ZigJointId.RightAnkle));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.Neck, ZigJointId.Head));
        sPairs.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.Torso, ZigJointId.Neck));
        //global waist angle??
    }

    

    public static float grade_to_perfect(float aGrade)
    {
		//0 is bad 1 is perfect
		float maxgn = GameConstants.maxGradeNorm;
		float mingn = GameConstants.minGradeNorm;
        return Mathf.Clamp((maxgn - aGrade - mingn) / (maxgn-mingn), 0, 1);
    }
	
	public static float grade_joint(Pose A, Pose B, ZigJointId aJoint)
	{
		PoseElement e = A.find_element(aJoint);
		PoseElement bPose = B.find_element(aJoint);
		float target = bPose.angle;
        float actual = e.angle;
        float diff = target - actual;
        while (diff > 180) diff -= 360;
        while(diff < -180) diff += 360;
		return diff;
	}
	
	public Dictionary<ZigJointId,float> advanced_grade_pose(Pose A, Pose B)
	{
		Dictionary<ZigJointId,float> r = new Dictionary<ZigJointId, float>();
		
		float weightsum = 0;
        float gradesum = 0;
        string output = "";
		foreach (PoseElement e in A.mElements)
            weightsum += B.find_element(e.joint).weight;
        foreach (PoseElement e in A.mElements)
        {
            PoseElement bPose = B.find_element(e.joint);
            float target = bPose.angle;
            float actual = e.angle;
            float diff = target - actual;
            while (diff > 180) diff -= 360;
            while(diff < -180) diff += 360;
            r[e.joint] = diff * diff * bPose.weight; ///weightSum;
        }
        return r;
	}
	
    public static float grade_pose(Pose A, Pose B) //weight is taken from Pose B, B is traget
    {
        float weightsum = 0;
        float gradesum = 0;
        string output = "";
        foreach (PoseElement e in A.mElements)
        {
            //Debug.Log(e.joint);
            PoseElement bPose = B.find_element(e.joint);
            float target = bPose.angle;
            float actual = e.angle;
            float diff = target - actual;
            while (diff > 180) diff -= 360;
            while(diff < -180) diff += 360;
            gradesum += diff * diff * bPose.weight;
            weightsum += bPose.weight;
            output += e.joint + " target: " + target + " actual: " + actual + " diff: " + diff + "\n";
        }
        output += " grade: " + Mathf.Sqrt(gradesum) / weightsum;
        return Mathf.Sqrt(gradesum) / weightsum;
    }
    public static Pose snap_pose(ManagerManager manager)
    {
        Pose p = new Pose();
        foreach (KeyValuePair<ZigJointId, ZigJointId> e in sPairs)
        {
            PoseElement pe = new PoseElement();
            //ZigInputJoint A = manager.mZigManager.Joints[e.Key];
            //ZigInputJoint B = manager.mZigManager.Joints[e.Value];
            //pe.angle = manager.mProjectionManager.get_relative(A, B);
            pe.weight = 1;
            pe.angle = manager.mProjectionManager.get_smoothed_relative(e.Key, e.Value);
            pe.joint = e.Key;
            p.mElements.Add(pe);
        }
        PoseElement waist = new PoseElement();
        waist.angle = manager.mProjectionManager.mWaist.current;
        //waist.angle = manager.mProjectionManager.get_waist(manager.mZigManager.Joints[ZigJointId.Waist], manager.mZigManager.Joints[ZigJointId.LeftHip], manager.mZigManager.Joints[ZigJointId.RightHip]);
        waist.joint = ZigJointId.Waist;
        p.mElements.Add(waist);
        return p;
    }
	//TODO should probably put this somewhere else..
	public static Pose from_file(string path)
	{
		Pose p = new Pose();
		System.IO.FileStream stream = new System.IO.FileStream(path,System.IO.FileMode.Open);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Pose));
        p = (Pose)xs.Deserialize(stream);
        return p;
	}
    public static Pose read_pose(TextAsset aText)
    {
        Pose p = new Pose();
        System.IO.MemoryStream stream = new System.IO.MemoryStream(aText.bytes);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Pose));
        p = (Pose)xs.Deserialize(stream);
        return p;
    }
    public static void write_pose_to_file(Pose p, string aFile)
    {
        System.IO.Stream stream = System.IO.File.Open(aFile, System.IO.FileMode.Create);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Pose));
        xs.Serialize(stream, p);
        stream.Close();
        Debug.Log("write file " + aFile);
    }
}