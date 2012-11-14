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
		Joints = new Dictionary<ZigJointId, ZigInputJoint>();
		//pfft, unity can't seem to compile this
		//foreach(ZigJointId e in Enum.GetValues(typeof(ZigJointId)))
		//	Joints[e] = new ZigInputJoint(e);
	}

	// Use this for initialization
	public override void Start () {
        mZigObject = mManager.gameObject;
        //mZigObject.AddComponent<kinectSpecific>();
		mZig = mZigObject.GetComponent<Zig>();
		mZigEngageSingleUser = mZigObject.GetComponent<ZigEngageSingleUser>();
        mZigCallbackBehaviour = mZigObject.AddComponent<ZigCallbackBehaviour>();
        
        mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
		mZigEngageSingleUser.EngagedUsers.Add(mManager.gameObject);
        mZigCallbackBehaviour.mUpdateUserDelegate += this.Zig_UpdateUser;
	}
	
	public override void Update () 
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(mManager.mGradingManager.print_pose());
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
            foreach (ZigInputJoint joint in user.Skeleton)
            {
                if(joint.GoodPosition && joint.GoodRotation)
                {
					Joints[joint.Id] = joint;
                }
            }
        }
    } 
}
