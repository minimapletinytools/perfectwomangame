using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}
    public FlatBodyObject mFlat;
    int mMode = 0; // 0 - from kinect, 1 - from pose, -1 none
    int mLayer = 0;
    Vector3 mOffset;
    public void set_target_pose(ProGrading.Pose aPose)
    {
        mMode = 1;
        mFlat.set_target_pose(aPose);
        fix_target_pose();
    }

    public void set_layer(int layer)
    {
        mLayer = layer;
        if (mFlat != null)
        {
            foreach (GameObject e in mFlat.mParts.Values)
            {
                foreach (Renderer f in e.GetComponentsInChildren<Renderer>())
                {
                    f.gameObject.layer = layer;
                }
            }
        }
    }

    public void destroy_character()
    {
        if (mFlat != null)
        {
            mFlat.destroy();
            mFlat = null; //make sure mFlat was not in some other container
        }
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        destroy_character();
        mFlat = new FlatBodyObject(aCharacter,-1);
        set_layer(mLayer);
        mFlat.HardPosition = Random.insideUnitCircle.normalized * 300000;
        mFlat.SoftPosition = Vector3.zero;
        mOffset = (new Vector3(BodyManager.convert_units(aCharacter.background1.width) / 4.0f, 0, 0) + BodyManager.convert_units(aCharacter.adjust));
        mFlat.SoftPosition = mFlat.SoftPosition + mOffset;
        mFlat.update(0);
    }


    public static Vector3 convert_units(Vector3 pixel)
    {
        return pixel / 1.0f;
    }
	public static float convert_units(double pixelWidth)
	{
		return (float)pixelWidth/1.0f; //100 pixels = 1 unit
	}

    public void fix_target_pose()
    {
        /*
        ProGrading.Pose pose = mFlat.mTargetPose;
        List<ProGrading.PoseElement> elements = pose.mElements;
        int waistIndex = -1; 
        int kneeIndex = -1;
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].joint == ZigJointId.Waist)
                waistIndex = i;
            if (elements[i].joint == ZigJointId.LeftHip)
                kneeIndex = i;
        }
        float measured = elements[waistIndex].angle + 100;
        int maxIter = 100;
        while(maxIter > 0 && Mathf.Abs(measured-elements[waistIndex].angle) > 0.1f)
        {
            if (maxIter == 100)
            {
                elements[waistIndex].angle = mManager.mProjectionManager.get_waist(mFlat.mParts[ZigJointId.Waist].transform.position, mFlat.mParts[ZigJointId.LeftKnee].transform.position, mFlat.mParts[ZigJointId.RightKnee].transform.position);
                measured = mManager.mProjectionManager.get_waist(mFlat.mParts[ZigJointId.Waist].transform.position, mFlat.mParts[ZigJointId.LeftKnee].transform.position, mFlat.mParts[ZigJointId.RightKnee].transform.position);
            }
            elements[kneeIndex].angle -= (measured - elements[waistIndex].angle) * 0.5f;
            pose.mElements = elements;
            mFlat.set_target_pose(pose);
            measured = mManager.mProjectionManager.get_waist(mFlat.mParts[ZigJointId.Waist].transform.position, mFlat.mParts[ZigJointId.LeftKnee].transform.position, mFlat.mParts[ZigJointId.RightKnee].transform.position);
            maxIter--;
        }
        pose.mElements = elements;
        mFlat.set_target_pose(pose);
        Debug.Log("fixed with iterations " + maxIter + " diff " + (measured - elements[waistIndex].angle));
         */
    }

	public override void Start () 
    {
        
        mManager.mEventManager.character_changed_event += character_changed_listener;
	}

    public override void Update()
    {
        if (mFlat != null)
        {
            mFlat.update_parameters(Time.deltaTime);
            mFlat.set();
            if (mMode == 0)
            {
                if (mManager.mGameManager.Started)
                {
                    Vector3 position = mManager.mZigManager.Joints[ZigJointId.Waist].Position;
                    position.z = 0;
                    mFlat.SoftPosition = position/1.5f + mOffset;
                }

                foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in mManager.mProjectionManager.mImportant)
                {
                    mFlat.mParts[e.Key].transform.rotation = Quaternion.AngleAxis(e.Value.smoothing.current, Vector3.forward);
                }
                mFlat.mParts[ZigJointId.Waist].transform.rotation = Quaternion.AngleAxis(mManager.mProjectionManager.mWaist.current, Vector3.forward);


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (mManager.mZigManager.has_user())
                        ProGrading.write_pose_to_file(ProGrading.snap_pose(mManager), "char_kinect.txt");
                }
            }
            else if (mMode == 1)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ProGrading.Pose p = new ProGrading.Pose();
                    foreach (KeyValuePair<ZigJointId, GameObject> e in mFlat.mParts)
                    {
                        ProGrading.PoseElement pe = new ProGrading.PoseElement();
                        pe.joint = e.Key;
                        pe.angle = e.Value.transform.rotation.eulerAngles.z;
                        p.mElements.Add(pe);
                    }
                    ProGrading.write_pose_to_file(p, "char_manual.txt");
                }
            }
        }
        
	}

    public static ZigJointId get_parent(ZigJointId joint)
    {
        switch (joint)
        {
            case ZigJointId.LeftKnee:
                return ZigJointId.LeftHip;
            case ZigJointId.RightKnee:
                return ZigJointId.RightHip;
            case ZigJointId.LeftHip:
                return ZigJointId.Waist;
            case ZigJointId.RightHip:
                return ZigJointId.Waist;
            case ZigJointId.LeftElbow:
                return ZigJointId.LeftShoulder;
            case ZigJointId.RightElbow:
                return ZigJointId.RightShoulder;
            case ZigJointId.LeftShoulder:
                return ZigJointId.Torso;
            case ZigJointId.RightShoulder:
                return ZigJointId.Torso;
            case ZigJointId.Neck:
                return ZigJointId.Torso;
            default:
                return ZigJointId.None;
        }
    }
}
