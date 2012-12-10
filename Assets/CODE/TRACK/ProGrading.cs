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

    [System.Serializable]
    public class PoseElement
    {
        public ZigJointId joint;
        public float angle;
        public float weight = 1;
        public bool important = true; //this means we care about angle
        //other info if we want it...
    }

    [System.Serializable]
    public class Pose
    {
        public List<PoseElement> mElements = new List<PoseElement>();
        public PoseElement find_element(ZigJointId id)
        {
            foreach (PoseElement e in mElements)
                if (e.joint == id)
                    return e;
            throw new UnityException("can't find ZigJointId " + id + " in Pose");
        }
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
        if (Input.GetKeyDown(KeyCode.D))
            Debug.Log(output);
        return Mathf.Sqrt(gradesum) / weightsum;
    }
    public static Pose snap_pose(ManagerManager manager) //make suree there is a skeleton to snap
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