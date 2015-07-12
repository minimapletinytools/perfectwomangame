using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ProGrading {
    public static List<KeyValuePair<ZgJointId, ZgJointId>> sPairs;
    static ProGrading()
    {
        sPairs = new List<KeyValuePair<ZgJointId, ZgJointId>>();
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftShoulder, ZgJointId.LeftElbow));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftElbow, ZgJointId.LeftHand));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftHip, ZgJointId.LeftKnee));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftKnee, ZgJointId.LeftAnkle));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightShoulder, ZgJointId.RightElbow));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightElbow, ZgJointId.RightHand));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightHip, ZgJointId.RightKnee));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightKnee, ZgJointId.RightAnkle));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.Neck, ZgJointId.Head));
        sPairs.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.Torso, ZgJointId.Neck));
        //global waist angle??
    }

    

    public static float grade_to_perfect(float aGrade)
    {
		//0 is bad 1 is perfect
		float maxgn = GameConstants.maxGradeNorm;
		float mingn = GameConstants.minGradeNorm;
        return Mathf.Clamp((maxgn - aGrade - mingn) / (maxgn-mingn), 0, 1);
    }
	
	public static float grade_joint(Pose A, Pose B, ZgJointId aJoint)
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
	
	public static Dictionary<ZgJointId,float> advanced_grade_pose(Pose A, Pose B)
	{
		Dictionary<ZgJointId,float> r = new Dictionary<ZgJointId, float>();
		
		float weightSum = 0;
		foreach (PoseElement e in A.mElements)
            weightSum += B.find_element(e.joint).weight;
        foreach (PoseElement e in A.mElements)
        {
            PoseElement bPose = B.find_element(e.joint);
            float target = bPose.angle;
            float actual = e.angle;
            float diff = target - actual;
            while (diff > 180) diff -= 360;
            while(diff < -180) diff += 360;
            r[e.joint] = diff;
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
        foreach (KeyValuePair<ZgJointId, ZgJointId> e in sPairs)
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
        waist.joint = ZgJointId.Waist;
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

    public static ZgJointId parse_ZigJointId_string(string aString)
    {
        if (aString == "Head")
            return ZgJointId.Head;
        else if (aString == "Neck")
            return ZgJointId.Neck;
        else if (aString == "Torso")
            return ZgJointId.Torso;
        else if (aString == "Waist")
            return ZgJointId.Waist;
        else if (aString == "LeftCollar")
            return ZgJointId.LeftCollar;
        else if (aString == "LeftShoulder")
            return ZgJointId.LeftShoulder;
        else if (aString == "LeftElbow")
            return ZgJointId.LeftElbow;
        else if (aString == "LeftWrist")
            return ZgJointId.LeftWrist;
        else if (aString == "LeftHand")
            return ZgJointId.LeftHand;
        else if (aString == "LeftFingertip")
            return ZgJointId.LeftFingertip;
        else if (aString == "RightCollar")
            return ZgJointId.RightCollar;
        else if (aString == "RightShoulder")
            return ZgJointId.RightShoulder;
        else if (aString == "RightElbow")
            return ZgJointId.RightElbow;
        else if (aString == "RightWrist")
            return ZgJointId.RightWrist;
        else if (aString == "RightHand")
            return ZgJointId.RightHand;
        else if (aString == "RightFingertip")
            return ZgJointId.RightFingertip;
        else if (aString == "LeftHip")
            return ZgJointId.LeftHip;
        else if (aString == "LeftKnee")
            return ZgJointId.LeftKnee;
        else if (aString == "LeftAnkle")
            return ZgJointId.LeftAnkle;
        else if (aString == "LeftFoot")
            return ZgJointId.LeftFoot;
        else if (aString == "RightHip")
            return ZgJointId.RightHip;
        else if (aString == "RightKnee")
            return ZgJointId.RightKnee;
        else if (aString == "RightAnkle")
            return ZgJointId.RightAnkle;
        else if (aString == "RightFoot")
            return ZgJointId.RightFoot;
        else
            throw new UnityException("no such joint " + aString);
    }
    public static Pose read_pose(TextAsset aText)
    {
        return read_pose(aText.text);
    }

    public static Pose read_pose(string aPose)
    {
        Pose p = new Pose();
        //System.IO.MemoryStream stream = new System.IO.MemoryStream(aText.bytes);
        var tr = new System.IO.StringReader(aPose); 
        var xr = System.Xml.XmlReader.Create(tr);
        var xd = new System.Xml.XmlDocument();
        xd.Load(xr);
        
        var elts = xd.ChildNodes [1].FirstChild;
        for(int i = 0; i < elts.ChildNodes.Count; i++)
        {
            var active = elts.ChildNodes[i];
            var activeElement = new PoseElement();
            for(int j = 0; j < active.ChildNodes.Count; j++)
            {
                if(active.ChildNodes[j].Name == "joint")
                    activeElement.joint = parse_ZigJointId_string(active.ChildNodes[j].InnerXml);
                else if(active.ChildNodes[j].Name == "angle")
                    activeElement.angle = float.Parse(active.ChildNodes[j].InnerXml);
            }
            p.mElements.Add(activeElement);
        }
        //System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Pose));
        //p = (Pose)xs.Deserialize(stream);
        //p = (Pose)xs.Deserialize(tr);
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