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

    public void character_changed_listener(CharacterLoader aCharacter)
    {
        destroy_character();

        if (aCharacter != null)
        {
            mFlat = new FlatBodyObject(aCharacter, -1);
            set_layer(mLayer);
            if (ManagerManager.Manager.mGameManager.CurrentLevel == 0)
                mFlat.HardPosition = Random.insideUnitCircle.normalized * 30000;
            else
                mFlat.HardPosition = Random.insideUnitCircle.normalized * 1000;
            mFlat.SoftPosition = Vector3.zero;
            mOffset = (new Vector3(aCharacter.Sizes.mOffset.x, aCharacter.Sizes.mOffset.y,0));
            mFlat.SoftPosition = mFlat.SoftPosition + mOffset;
            mFlat.update(0);
        }
    }


    public static Vector3 convert_units(Vector3 pixel)
    {
        return pixel / 1.0f;
    }
	public static float convert_units(double pixelWidth)
	{
		return (float)pixelWidth/1.0f; //100 pixels = 1 unit
	}

	//TODO delete
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


    int write_counter = 0;
    public override void Update()
    {
        if (mFlat != null)
        {
            mFlat.update_parameters(Time.deltaTime);
            mFlat.set();
            if (mMode == 0)
            {

                if (ManagerManager.Manager.mGameManager.Started)
                {
                    Vector3 position = Vector3.zero;
                    if (ManagerManager.Manager.mZigManager.Joints.ContainsKey(ZigJointId.Waist)) //lol you can replace this with try get value... actually I fixed this so just delete this dumb check
                        position = ManagerManager.Manager.mZigManager.Joints[ZigJointId.Waist].Position;
                    position.z = 0;
                    position.y = 0;
                    position.x *= -1;
                    if (Mathf.Abs(position.x) < 10) position.x = 0; //fake snapping, TODO this should probbaly be in grading manager if anywhere...
                    mFlat.SoftPosition = position / 1.5f + mOffset;
                }
                match_body_to_projection(mFlat);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (mManager.mZigManager.has_user())
                    {
                        write_counter++;
                        ProGrading.write_pose_to_file(ProGrading.snap_pose(mManager), "char_kinect"+write_counter+".txt");
                    }
                }
            }
            else if (mMode == 1)
            {
                if (Input.GetKeyDown(KeyCode.Space)) //TODO manual pose setting wont work anymore due to targetting on target pose.
                {
                    write_counter++;
                    ProGrading.Pose p = new ProGrading.Pose();
                    foreach (KeyValuePair<ZigJointId, GameObject> e in mFlat.mParts)
                    {
                        ProGrading.PoseElement pe = new ProGrading.PoseElement();
                        pe.joint = e.Key;
                        pe.angle = e.Value.transform.rotation.eulerAngles.z;
                        p.mElements.Add(pe);
                    }
                    ProGrading.write_pose_to_file(p, "char_manual" + write_counter + ".txt");
                }
            }
        }
        
	}

    public static void match_body_to_projection(FlatBodyObject aObject)
    {
        foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in ManagerManager.Manager.mProjectionManager.mImportant)
        {
            aObject.mParts[e.Key].transform.rotation = Quaternion.AngleAxis(e.Value.smoothing.current, Vector3.forward);
        }
        aObject.mParts[ZigJointId.Waist].transform.rotation = Quaternion.AngleAxis(ManagerManager.Manager.mProjectionManager.mWaist.current, Vector3.forward);
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
