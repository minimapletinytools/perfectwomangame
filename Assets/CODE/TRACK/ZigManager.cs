using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ZigManager : FakeMonoBehaviour {
    public ZigInterface ZigInterface { get; private set; }
	public AlternativeDepthViewer DepthView { get; private set; }
	public AlternativeImageViewer ImageView { get; private set; }
    public Dictionary<ZigJointId, ZigInputJoint> Joints{get; private set;}
	ZigJointId[] ImportantJoints = new ZigJointId[]{ZigJointId.Head,ZigJointId.LeftHand,ZigJointId.RightHand};//,ZigJointId.LeftAnkle,ZigJointId.RightAnkle};
    public ZigTrackedUser LastTrackedUser { get; private set; }
    public ZigManager(ManagerManager aManager) : base(aManager)
	{
        ZigInterface = new EmptyZig();
        ZigInterface.initialize(this);
		Joints = new Dictionary<ZigJointId, ZigInputJoint>()
		{
			{ZigJointId.Head,new ZigInputJoint(ZigJointId.Head)},
			{ZigJointId.Torso,new ZigInputJoint(ZigJointId.Torso)},
			{ZigJointId.Waist,new ZigInputJoint(ZigJointId.Waist)},
			{ZigJointId.LeftShoulder,new ZigInputJoint(ZigJointId.LeftShoulder)},
			{ZigJointId.LeftElbow,new ZigInputJoint(ZigJointId.LeftElbow)},
			{ZigJointId.LeftHand,new ZigInputJoint(ZigJointId.LeftHand)},
			{ZigJointId.LeftHip,new ZigInputJoint(ZigJointId.LeftHip)},
			{ZigJointId.LeftKnee,new ZigInputJoint(ZigJointId.LeftKnee)},
			{ZigJointId.LeftAnkle,new ZigInputJoint(ZigJointId.LeftAnkle)},
			{ZigJointId.RightShoulder,new ZigInputJoint(ZigJointId.RightShoulder)},
			{ZigJointId.RightElbow,new ZigInputJoint(ZigJointId.RightElbow)},
			{ZigJointId.RightHand,new ZigInputJoint(ZigJointId.RightHand)},
			{ZigJointId.RightHip,new ZigInputJoint(ZigJointId.RightHip)},
			{ZigJointId.RightKnee,new ZigInputJoint(ZigJointId.RightKnee)},
			{ZigJointId.RightAnkle,new ZigInputJoint(ZigJointId.RightAnkle)},
		};
		//pfft, unity can't seem to compile this
		//foreach(ZigJointId e in Enum.GetValues(typeof(ZigJointId)))
		//	Joints[e] = new ZigInputJoint(e);
	}

	// Use this for initialization
	public override void Start () {
       
        DepthView = mManager.gameObject.AddComponent<AlternativeDepthViewer>();
        ImageView = mManager.gameObject.AddComponent<AlternativeImageViewer>();
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
		
        ZigInterface.update();
	}
	
	public int is_reader_connected() //0 - not connected, 1 - trying to connect, 2 - connected
	{
        if(ZigInterface.ZigInput == null)
			return 1;
        else if(ZigInterface.ZigInput.ReaderInited == true)
			return 2;
		else return 0;
	}

    public bool using_nite()
    {
        if (ZigInterface.ZigInput != null)
        {
            return !ZigInterface.ZigInput.kinectSDK;
        }

        //default is true because it is a safer choice
        return true;
    }
	
	public bool has_user()
	{
        return ZigInterface.has_user();
	}

    Quaternion get_relative_rotation(ZigInputJoint A, ZigInputJoint B)
    {
        return get_relative_rotation(A, B, new Vector3(1, 0, 0));
    }
    Quaternion get_relative_rotation(ZigInputJoint A, ZigInputJoint B, Vector3 aRelative)
    {
        Vector3 v = B.Position - A.Position;
        return Quaternion.FromToRotation(aRelative, v);
    }

    //calls this function whenever user info has been updated
	public void Zig_UpdateUser(ZigTrackedUser user)
    {
        LastTrackedUser = user;
        if (user.SkeletonTracked)
        {
			string output = "";
            foreach (ZigInputJoint joint in user.Skeleton)
            {
                //if(joint.GoodPosition && joint.GoodRotation)
                {
					
					Joints[joint.Id] = joint;
                }
            }
			//Debug.Log(output);
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
		if(Joints.ContainsKey(ZigJointId.LeftShoulder) && 
		   Joints.ContainsKey(ZigJointId.LeftElbow) && 
		   Joints.ContainsKey(ZigJointId.Neck) && 
		   Joints.ContainsKey(ZigJointId.Head))
			if(get_relative_rotation(Joints[ZigJointId.LeftShoulder],Joints[ZigJointId.LeftElbow]).flat_rotation() == 0 &&
			   get_relative_rotation(Joints[ZigJointId.Neck],Joints[ZigJointId.Head]).flat_rotation() == 0)
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
