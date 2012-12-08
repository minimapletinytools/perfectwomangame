using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}
    public FlatBodyObject mFlat;
    int mMode = 0; // 0 - from kinect, 1 - from pose, -1 none
    int mLayer = 0;
    public void set_target_pose(ProGrading.Pose aPose)
    {
        mMode = 1;
        mFlat.set_target_pose(aPose);
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
        mFlat = new FlatBodyObject(aCharacter);
        set_layer(mLayer);
        mFlat.HardPosition = Random.insideUnitCircle.normalized * 300000;
        mFlat.SoftPosition = Vector3.zero;
        mFlat.SoftPosition = mFlat.SoftPosition + (new Vector3(BodyManager.convert_units(aCharacter.background1.width) / 4.0f, 0, 0) + BodyManager.convert_units(aCharacter.adjust));
    }


    public static Vector3 convert_units(Vector3 pixel)
    {
        return pixel / 1.0f;
    }
	public static float convert_units(double pixelWidth)
	{
		return (float)pixelWidth/1.0f; //100 pixels = 1 unit
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
                //TODO move this to grading manager fool
                if (mManager.mZigManager.has_user())
                    mManager.mInterfaceManager.mGrade = ProGrading.grade_pose(ProGrading.snap_pose(mManager), mManager.mTransparentBodyManager.mFlat.mTargetPose);

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
                if (mFlat.mTargetPose != null && mManager.mRecordMode == false)
                {
                    foreach (ProGrading.PoseElement e in mFlat.mTargetPose.mElements)
                    {
                        mFlat.mParts[e.joint].transform.rotation = Quaternion.AngleAxis(e.angle, Vector3.forward);
                    }
                }
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
