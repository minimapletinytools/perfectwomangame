using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}
    public FlatBodyObject mFlat;
    public int mMode = 0; // 0 - from kinect, 1 - from pose, -1 none
    int mLayer = 0;
    Vector3 mOffset;
	
    public void set_target_pose(Pose aPose, bool hard = false)
    {
        mFlat.set_target_pose(aPose, hard);
        //hack because the hard parameter above does not seem to work...
		if(hard)
		{
			for (int i = 0; i < 100; i++)
            {
                mFlat.update_parameters(100);
            }
		}
		//hack hack
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
	
	
	public Pose get_current_pose()
	{
		return mFlat.get_pose();
	}
	
    public void set_layer(int layer)
    {
        mLayer = layer;
        if (mFlat != null)
			mFlat.set_layer(mLayer);
    }
	

	public void transition_character_in(Color aColor)
	{
		if(mFlat != null)
		{
			mFlat.SoftColor = aColor;
			mFlat.PositionInterpolationMaxLimit = 5000;
			mFlat.SoftPosition = mOffset;
		}
	}
	public void transition_character_out(bool aHard = false)
	{
		//fade out
		//mFlat.SoftColor = new Color(1,1,1,0);

		//fly out
		mFlat.PositionInterpolationMaxLimit = 5000;
        if(!aHard)
		    mFlat.SoftPosition = mFlat.HardPosition + new Vector3(0,4000,0);
        else
            mFlat.HardPosition = mFlat.SoftPosition = mFlat.HardPosition + new Vector3(0,4000,0);
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
    public void load_character(CharacterLoader aCharacter)
    {
        destroy_character();

        if (aCharacter != null && aCharacter.Name != "999")
        {
            mFlat = new FlatBodyObject(aCharacter, 30);
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
				//mFlat.SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
				mFlat.HardShader = mManager.mReferences.mTransparentCharacaterShader;
				mFlat.HardColor = mManager.mCharacterBundleManager.get_character_stat(aCharacter.Character).CharacterInfo.CharacterOutlineColor;
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

    public void write_pose(string aFilename, bool aManual)
    {
        if (aManual)
        {
            ProGrading.write_pose_to_file(get_current_pose(), aFilename);
        }
        else
        {
            //if (mManager.mZigManager.has_user())
            {
                ProGrading.write_pose_to_file(ProGrading.snap_pose(mManager),aFilename);
            }
        }
    }


    public override void Update()
    {
        if (mFlat != null)
        {
			/* TODO DELETE we set the pose elsewhere now
            //if were not in record mode for the tranpsarent one
            if ( mMode == 1)
            {
                mFlat.update_parameters(Time.deltaTime);
                mFlat.set();
            }
            else if (mMode == 0)
            {
				
				//only update if kinect is pulgged in
				if(ManagerManager.Manager.mZigManager.is_reader_connected() == 2)
				{
					//TODO do I need this??
	                //if (ManagerManager.Manager.mGameManager.Started)
	                {
	                    //mFlat.match_body_location_to_projection(mManager.mZigManager);
	                }
					
					
					if(!Input.GetKey(KeyCode.A))//hack 
	                	mFlat.match_body_to_projection(mManager.mProjectionManager);
				}
				mFlat.update_parameters(Time.deltaTime);
	            mFlat.set();
            }
			*/
			
			mFlat.update_parameters(Time.deltaTime);
	            mFlat.set();
        }
        
	}
	
	public void keyboard_update()
	{
		if(mFlat != null){
			//US
			/*var keyBindings = new[]{
				new { k = ZigJointId.Neck, u = KeyCode.T, d = KeyCode.Y },
				new { k = ZigJointId.Torso, u = KeyCode.G, d = KeyCode.H },
				new { k = ZigJointId.Waist, u = KeyCode.B, d = KeyCode.N },
				new { k = ZigJointId.LeftShoulder, u = KeyCode.E, d = KeyCode.R },
				new { k = ZigJointId.RightShoulder, u = KeyCode.U, d = KeyCode.I },
				new { k = ZigJointId.LeftElbow, u = KeyCode.Q, d = KeyCode.W },
				new { k = ZigJointId.RightElbow, u = KeyCode.O, d = KeyCode.P },
				new { k = ZigJointId.LeftHip, u = KeyCode.C, d = KeyCode.V },
				new { k = ZigJointId.RightHip, u = KeyCode.M, d = KeyCode.Comma },
				new { k = ZigJointId.LeftKnee, u = KeyCode.Z, d = KeyCode.X },
				new { k = ZigJointId.RightKnee, u = KeyCode.Period, d = KeyCode.Slash }
			};*/
			//GERMAN
			var keyBindings = new[]{
				new { k = ZgJointId.Neck, u = KeyCode.T, d = KeyCode.Z },
				new { k = ZgJointId.Torso, u = KeyCode.G, d = KeyCode.H },
				new { k = ZgJointId.Waist, u = KeyCode.B, d = KeyCode.N },
				new { k = ZgJointId.LeftShoulder, u = KeyCode.E, d = KeyCode.R },
				new { k = ZgJointId.RightShoulder, u = KeyCode.U, d = KeyCode.I },
				new { k = ZgJointId.LeftElbow, u = KeyCode.Q, d = KeyCode.W },
				new { k = ZgJointId.RightElbow, u = KeyCode.O, d = KeyCode.P },
				new { k = ZgJointId.LeftHip, u = KeyCode.C, d = KeyCode.V },
				new { k = ZgJointId.RightHip, u = KeyCode.M, d = KeyCode.Comma },
				new { k = ZgJointId.LeftKnee, u = KeyCode.Y, d = KeyCode.X },
				new { k = ZgJointId.RightKnee, u = KeyCode.Period, d = KeyCode.Minus }
			};
			Pose p = new Pose();
		    foreach (KeyValuePair<ZgJointId, GameObject> e in mFlat.mParts)
		    {
		        PoseElement pe = new PoseElement();
		        pe.joint = e.Key;
		        pe.angle = e.Value.transform.rotation.eulerAngles.z;
		        p.mElements.Add(pe);
		    }
			foreach(var e in keyBindings)
			{
				if(Input.GetKey(e.u))
					p.find_element(e.k).angle += 1;
				else if(Input.GetKey(e.d))
					p.find_element(e.k).angle -= 1;
			}
			
			
			mFlat.set_target_pose(p,true);
		}
	}

    public static ZgJointId get_parent(ZgJointId joint)
    {
        switch (joint)
        {
            case ZgJointId.LeftKnee:
                return ZgJointId.LeftHip;
            case ZgJointId.RightKnee:
                return ZgJointId.RightHip;
            case ZgJointId.LeftHip:
                return ZgJointId.Waist;
            case ZgJointId.RightHip:
                return ZgJointId.Waist;
            case ZgJointId.LeftElbow:
                return ZgJointId.LeftShoulder;
            case ZgJointId.RightElbow:
                return ZgJointId.RightShoulder;
            case ZgJointId.LeftShoulder:
                return ZgJointId.Torso;
            case ZgJointId.RightShoulder:
                return ZgJointId.Torso;
            case ZgJointId.Neck:
                return ZgJointId.Torso;
            default:
                return ZgJointId.None;
        }
    }
}
