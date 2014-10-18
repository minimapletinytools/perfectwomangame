using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ZgManager : FakeMonoBehaviour {
    public ZgInterface ZgInterface { get; private set; }

	public ZgDepthViewer DepthView { get; private set; }
	public ZgImageViewer ImageView { get; private set; }

    public Dictionary<ZgJointId, ZgInputJoint> Joints{get; private set;}
	ZgJointId[] ImportantJoints = new ZgJointId[]{ZgJointId.Head,ZgJointId.LeftHand,ZgJointId.RightHand};//,ZigJointId.LeftAnkle,ZigJointId.RightAnkle};
    public ZgTrackedUser LastTrackedUser { get; private set; }
    public ZgManager(ManagerManager aManager) : base(aManager)
	{
        //ZgInterface = new EmptyZig();
#if UNITY_XBOXONE && !UNITY_EDITOR
		ZgInterface = new MicrosoftZig();
#else
		ZgInterface = new ZigFuZig();
#endif


        ZgInterface.initialize(this);
		Joints = new Dictionary<ZgJointId, ZgInputJoint>()
		{
			{ZgJointId.Head,new ZgInputJoint(ZgJointId.Head)},
			{ZgJointId.Torso,new ZgInputJoint(ZgJointId.Torso)},
			{ZgJointId.Waist,new ZgInputJoint(ZgJointId.Waist)},
			{ZgJointId.LeftShoulder,new ZgInputJoint(ZgJointId.LeftShoulder)},
			{ZgJointId.LeftElbow,new ZgInputJoint(ZgJointId.LeftElbow)},
			{ZgJointId.LeftHand,new ZgInputJoint(ZgJointId.LeftHand)},
			{ZgJointId.LeftHip,new ZgInputJoint(ZgJointId.LeftHip)},
			{ZgJointId.LeftKnee,new ZgInputJoint(ZgJointId.LeftKnee)},
			{ZgJointId.LeftAnkle,new ZgInputJoint(ZgJointId.LeftAnkle)},
			{ZgJointId.RightShoulder,new ZgInputJoint(ZgJointId.RightShoulder)},
			{ZgJointId.RightElbow,new ZgInputJoint(ZgJointId.RightElbow)},
			{ZgJointId.RightHand,new ZgInputJoint(ZgJointId.RightHand)},
			{ZgJointId.RightHip,new ZgInputJoint(ZgJointId.RightHip)},
			{ZgJointId.RightKnee,new ZgInputJoint(ZgJointId.RightKnee)},
			{ZgJointId.RightAnkle,new ZgInputJoint(ZgJointId.RightAnkle)},
		};
		//pfft, unity can't seem to compile this
		//foreach(ZigJointId e in Enum.GetValues(typeof(ZigJointId)))
		//	Joints[e] = new ZigInputJoint(e);
	}

	// Use this for initialization
	public override void Start () {
       
        DepthView = new ZgDepthViewer();
        ImageView = new ZgImageViewer();
		ForceShow = 0;
        
	}
	
	
	public int ForceShow {get;set;} //0 default, 1 forceshow, 2 noshow
	public override void Update () 
	{
		if(Input.GetKeyDown(KeyCode.K))
			ForceShow = (ForceShow + 1)%3;
		
		if(ForceShow == 1 || 
			(ForceShow != 2 && (is_reader_connected() == 2 && !is_user_in_screen())))
		{
			DepthView.show_indicator(true);
			mManager.mTransitionCameraManager.EnableDepthWarning = true;
		}
		else 
		{
			DepthView.show_indicator(false);
			mManager.mTransitionCameraManager.EnableDepthWarning = false;
		}
		
        ZgInterface.update();
        DepthView.update();
	}
	
	public int is_reader_connected() //0 - not connected, 1 - trying to connect, 2 - connected
	{
        if (ZgInterface.ReaderInitialized)
            return 2;
        return 0;
	}

    public bool using_nite()
    {
        return !ZgInterface.IsMicrosoftKinectSDK;
    }
	
	public bool has_user()
	{
        return ZgInterface.has_user();
	}

    Quaternion get_relative_rotation(ZgInputJoint A, ZgInputJoint B)
    {
        return get_relative_rotation(A, B, new Vector3(1, 0, 0));
    }
    Quaternion get_relative_rotation(ZgInputJoint A, ZgInputJoint B, Vector3 aRelative)
    {
        Vector3 v = B.Position - A.Position;
        return Quaternion.FromToRotation(aRelative, v);
    }

    //calls this function whenever user info has been updated
	public void Zig_UpdateUser(ZgTrackedUser user)
    {
        LastTrackedUser = user;
        if (user.SkeletonTracked)
        {
            foreach (ZgInputJoint joint in user.Skeleton)
            {
                //if(joint.GoodPosition && joint.GoodRotation)
                {
					
					Joints[joint.Id] = joint;
                }
            }
        }
		
		
		//mManager.mDebugString = Joints[ZigJointId.LeftHand].Position.ToString();
    } 
	
	public UnityEngine.Bounds get_user_bounds()
	{
		
		Bounds? r = null;
		//TODO
		foreach(var e in Joints)
		{
			if(!r.HasValue)
				r = e.Value.Position.to_bounds();
			r = r.Value.union(e.Value.Position);
		}
		return r.Value;
	}
	
	public bool is_user_centered()
	{
		//TODO
		//ManagerManager.Manager.mDebugString = get_user_bounds().center.ToString();
		
		
		return true;
	}

	//for openni, we use an alternative version because the openni one suckso
	public bool is_skeleton_tracked_alternative()
	{
		if (LastTrackedUser != null)
		{
			if (LastTrackedUser.SkeletonTracked == false || LastTrackedUser.PositionTracked == false)
				return false;
		} else return false;

		
		//TODO test if its in current "crumpled" pose, needed for OpenNI
		//instead we chec	k neck and one arm)
		if(Joints.ContainsKey(ZgJointId.LeftShoulder) && 
		   Joints.ContainsKey(ZgJointId.LeftElbow) && 
		   Joints.ContainsKey(ZgJointId.Neck) && 
		   Joints.ContainsKey(ZgJointId.Head))
			if(get_relative_rotation(Joints[ZgJointId.LeftShoulder],Joints[ZgJointId.LeftElbow]).flat_rotation() == 0 &&
			   get_relative_rotation(Joints[ZgJointId.Neck],Joints[ZgJointId.Head]).flat_rotation() == 0)
		{
			return false;
		}

		return true;
	}


	float badTimer = 0;
	public bool is_user_in_screen()
	{
		bool bad = false;
		if(!is_skeleton_tracked_alternative())
		{
			bad = true;
			badTimer = 0;
		}

		foreach(var e in Joints)
		{
			if(ImportantJoints.Contains(e.Key) && !e.Value.GoodPosition)
			{
				bad = true;
			}
		}

		if(!bad)
			badTimer = 1.0f;
		else
			badTimer -= Time.deltaTime;
		return badTimer > 0;
	}
	
}
