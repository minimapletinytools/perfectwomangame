using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//the callback behaviour in this cas e
public class ZigFuZig : ZgInterface
{
	//TODO DELETE
	public static Dictionary<ZigJointId,ZgJointId> sJointTypeMap = new Dictionary<ZigJointId,ZgJointId>()
	{
		{ZigJointId.LeftShoulder,ZgJointId.LeftShoulder},
		{ZigJointId.LeftElbow,ZgJointId.LeftElbow},
		{ZigJointId.LeftHip,ZgJointId.LeftHip},
		{ZigJointId.LeftKnee,ZgJointId.LeftKnee},
		{ZigJointId.LeftAnkle,ZgJointId.LeftAnkle},
		{ZigJointId.RightShoulder,ZgJointId.RightShoulder},
		{ZigJointId.RightElbow,ZgJointId.RightElbow},
		{ZigJointId.RightHip,ZgJointId.RightHip},
		{ZigJointId.RightKnee,ZgJointId.RightKnee},
		{ZigJointId.RightAnkle,ZgJointId.RightAnkle},
		{ZigJointId.Neck,ZgJointId.Neck},
		{ZigJointId.Torso,ZgJointId.Torso},
		{ZigJointId.Waist,ZgJointId.Waist},
		{ZigJointId.Head,ZgJointId.Head},
		{ZigJointId.LeftHand,ZgJointId.LeftHand},
		{ZigJointId.RightHand,ZgJointId.RightHand}
		
	};
	
	
	
	Zig mZig = null;
	ZigEngageSingleUser mZigEngageSingleUser = null;
	ZigCallbackBehaviour mZigCallbackBehaviour = null;
    ZigInput mZigInput = null;
    
    public void initialize(ZgManager aZig)
    {
        
        var mZigObject = ManagerManager.Manager.gameObject;
        //mZigObject.AddComponent<kinectSpecific>();
        mZig = mZigObject.GetComponent<Zig>();
        
		//TODO try this again...
        /*
        mZig = mZigObject.AddComponent<Zig>();
        mZig.inputType = ZigInputType.Auto;
        mZig.settings.UpdateDepth = true;
        mZig.settings.UpdateImage = true;
        mZig.settings.AlignDepthToRGB = false;
        mZig.settings.OpenNISpecific.Mirror = true;
        mZigObject.AddComponent<ZigEngageSingleUser>();
        */
        
        
        //ZigEngageSingleUser scans for all users but only reports results from one of them (the first I guess)
        //normally this is set in editor initializers but we don't do that here
        mZigEngageSingleUser = mZigObject.GetComponent<ZigEngageSingleUser>();
        mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
        mZigEngageSingleUser.EngagedUsers.Add(mZigObject);
        
        //this is the only way to get callbacks from ZigEngageSingleUser
        mZigCallbackBehaviour = mZigObject.AddComponent<ZigCallbackBehaviour>();
        mZigCallbackBehaviour.mUpdateUserDelegate += Zig_UpdateUser;
		mZigCallbackBehaviour.mUpdateInputDelegate += Zig_UpdateInput;
    }

	static ZgInputJoint ToZgInputJoint(ZigInputJoint aJoint)
	{
		//return new ZgInputJoint (sJointTypeMap[aJoint.Id], aJoint.Position, aJoint.Rotation, aJoint.Inferred);
		//I'm pretty sure this conversion from ZigJointId to ZgJointId is fine
		var r = new ZgInputJoint ((ZgJointId)((int)aJoint.Id), aJoint.Position, aJoint.Rotation, aJoint.Inferred);
		r.GoodPosition = aJoint.GoodPosition;
		r.GoodRotation = aJoint.GoodRotation;
		return r;
	}

	public void Zig_UpdateUser(ZigTrackedUser user)
	{
		ZgTrackedUser zguser = new ZgTrackedUser(user.Id);
		zguser.Position = user.Position;
		zguser.PositionTracked = user.PositionTracked;
		zguser.SkeletonTracked = user.SkeletonTracked;
		zguser.Skeleton = user.Skeleton.Select (e => ToZgInputJoint (e)).ToArray();
		ManagerManager.Manager.mZigManager.Zig_UpdateUser (zguser);
	}

    public void Zig_UpdateInput(ZigInput aInput)
	{
		ManagerManager.Manager.mZigManager.DepthView.Zig_Update(ZgInput);
	}

    public bool has_user()
    {
        return mZigEngageSingleUser.engagedTrackedUser != null;
    }

	public bool can_start()
	{
		return true;
	}

    public void update()
    {
    }

    
    public Texture2D take_color_image()
    {
        if(ManagerManager.Manager.mZigManager.is_reader_connected() == 2)
        {
            return ManagerManager.Manager.mZigManager.ImageView.UpdateTexture (ColorImage,LabelMap);
            //Debug.Log ("updated image");
        }
        return null;
    }


    public ZgInput ZgInput
    {
        get
        {
            //I'm not sure why I can't just do this in the statr routine
            if (mZigInput == null)
            {
                GameObject container = GameObject.Find("ZigInputContainer");
                if (container != null)
                    mZigInput = container.GetComponent<ZigInput>();
				else return null;
                
                //this is important!, this is the only way to get output from ZigInput via mZigCallbackBehaviour
                mZigInput.AddListener(ManagerManager.Manager.gameObject);
            }
			ZgInput r = new ZgInput();
			if(ZigInput.Depth != null)
				ZgInput.Depth = new ZgDepth(ZigInput.Depth.xres,ZigInput.Depth.yres,ZigInput.Depth.data);
			if(ZigInput.Image != null)
				ZgInput.Image = new ZgImage(ZigInput.Image.xres,ZigInput.Image.yres,ZigInput.Image.data);
			if(ZigInput.LabelMap != null)
				ZgInput.LabelMap = new ZgLabelMap(ZigInput.LabelMap.xres,ZigInput.LabelMap.yres,ZigInput.LabelMap.data);
			r.kinectSDK = mZigInput.kinectSDK;
			r.ReaderInited = mZigInput.ReaderInited;
			return r;
        }
    }

	public ZgDepth DepthImage{get{ return ZgInput.Depth; }}
	public ZgImage ColorImage{get{ return ZgInput.Image; }}
	public ZgLabelMap LabelMap{get{ return ZgInput.LabelMap; }}
    public bool ReaderInitialized { get{ return ZgInput != null && ZgInput.ReaderInited; } }
    public bool IsMicrosoftKinectSDK { get{ return ZgInput == null || ZgInput.kinectSDK; } } 
}