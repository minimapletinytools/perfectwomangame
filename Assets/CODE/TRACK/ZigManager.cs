using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ZigManager : FakeMonoBehaviour {
	GameObject mZigObject = null;
	Zig mZig = null;
	ZigEngageSingleUser mZigEngageSingleUser = null;
    ZigCallbackBehaviour mZigCallbackBehaviour = null;
    public Dictionary<ZigJointId, ZigInputJoint> Joints{get; private set;}
    public ZigManager(ManagerManager aManager) : base(aManager)
	{
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
        mZigObject = mManager.gameObject;
        //mZigObject.AddComponent<kinectSpecific>();
		mZig = mZigObject.GetComponent<Zig>();
		
        
		//ZigEngageSingleUser scans for all users but only reports results from one of them (the first I guess)
		//normally this is set in editor initializers but we don't do that here
		mZigEngageSingleUser = mZigObject.GetComponent<ZigEngageSingleUser>();
        mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
		mZigEngageSingleUser.EngagedUsers.Add(mManager.gameObject);
		
		//this is the only way to get callbacks from ZigEngageSingleUser
		mZigCallbackBehaviour = mZigObject.AddComponent<ZigCallbackBehaviour>();
        mZigCallbackBehaviour.mUpdateUserDelegate += this.Zig_UpdateUser;
	}
	
	public override void Update () 
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(mManager.mGradingManager.print_pose());
        }
	}
	
	
	public bool has_user()
	{
		return mZigEngageSingleUser.engagedTrackedUser != null;
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
	
	void Zig_UpdateUser(ZigTrackedUser user)
    {
		
        if (user.SkeletonTracked)
        {
			string output = "";
            foreach (ZigInputJoint joint in user.Skeleton)
            {
                //if(joint.GoodPosition && joint.GoodRotation)
                {
					
					ZigInputJoint j;
					if(Joints.TryGetValue(joint.Id,out j))
					{
						if(joint.Position == j.Position)
							output += "p " + joint.Id + ", ";
						if(joint.Rotation == j.Rotation)
							output += "q " + joint.Id + ", ";
					}
							
					
					Joints[joint.Id] = joint;
                }
            }
			//Debug.Log(output);
        }
    } 
}
