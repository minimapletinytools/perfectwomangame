using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}
    public FlatBodyObject mFlat;
    public int mMode = 0; // 0 - from kinect, 1 - from pose, -1 none
    int mLayer = 0;
    Vector3 mOffset;
    public void set_target_pose(ProGrading.Pose aPose)
    {
        mFlat.set_target_pose(aPose);
        //hack
        if (((ManagerManager.Manager.mRecordMode) && mMode == 1))
        {
            for (int i = 0; i < 100; i++)
            {
                mFlat.update_parameters(100);
            }
            mFlat.HardColor = new Color32(255, 255, 255, 255);
            mFlat.SoftInterpolation = 1;
            mFlat.set();
        }

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
	
	
	public void transition_character_out()
	{
		mFlat.SoftColor = new Color(1,1,1,0);
	}
	
    public void destroy_character()
    {
        if (mFlat != null)
        {
            mFlat.destroy();
            mFlat = null; //make sure mFlat was not in some other container
        }
    }

    //TODO should call this directly and get rid of that stupid callback thing in events
    public void character_changed_listener(CharacterLoader aCharacter)
    {
        destroy_character();

        if (aCharacter != null && aCharacter.Name != "999")
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
			
			if(mMode == 0)
				mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			else if (mMode == 1)
			{
				mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
				mFlat.HardShader = mManager.mReferences.mTransparentCharacaterShader;
			}
			
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

	public override void Start () 
    {
        
        mManager.mEventManager.character_changed_event += character_changed_listener;
	}

    public void write_pose(string aFilename, bool aManual)
    {
        if (aManual)
        {
            ProGrading.Pose p = new ProGrading.Pose();
            foreach (KeyValuePair<ZigJointId, GameObject> e in mFlat.mParts)
            {
                ProGrading.PoseElement pe = new ProGrading.PoseElement();
                pe.joint = e.Key;
                pe.angle = e.Value.transform.rotation.eulerAngles.z;
                p.mElements.Add(pe);
            }
            ProGrading.write_pose_to_file(p, aFilename);
        }
        else
        {
            if (mManager.mZigManager.has_user())
            {
                ProGrading.write_pose_to_file(ProGrading.snap_pose(mManager),aFilename);
            }
        }
    }

    public override void Update()
    {
        if (mFlat != null)
        {
            //if were not in record mode for the tranpsarent one
            if (!((ManagerManager.Manager.mRecordMode) && mMode == 1))
            {
                mFlat.update_parameters(Time.deltaTime);
                mFlat.set();
            }
            else if (mMode == 0)
            {
				
				//TODO do I need this??
                //if (ManagerManager.Manager.mGameManager.Started)
                {
                    mFlat.match_body_location_to_projection(mManager.mZigManager);
                }
                mFlat.match_body_to_projection(mManager.mProjectionManager);
				mFlat.update_parameters(Time.deltaTime);
                mFlat.set();
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
