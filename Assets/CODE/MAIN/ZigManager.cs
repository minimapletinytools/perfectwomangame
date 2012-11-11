using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ZigManager : FakeMonoBehaviour {
	GameObject mZigObject = null;
	Zig mZig = null;
	ZigEngageSingleUser mZigEngageSingleUser = null;
    ZigUpdateUserBehaviour mUpdateUserBehaviour = null;
    public Dictionary<ZigJointId, ZigInputJoint> mJoints = new Dictionary<ZigJointId, ZigInputJoint>();
    public ZigManager(ManagerManager aManager) : base(aManager)
	{
	}

	// Use this for initialization
	public override void Start () {
        mZigObject = mManager.gameObject;
        //mZigObject.AddComponent<kinectSpecific>();
		mZig = mZigObject.GetComponent<Zig>();
		mZigEngageSingleUser = mZigObject.GetComponent<ZigEngageSingleUser>();
        mUpdateUserBehaviour = mZigObject.AddComponent<ZigUpdateUserBehaviour>();
        
        mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
		mZigEngageSingleUser.EngagedUsers.Add(mManager.gameObject);
        mUpdateUserBehaviour.mUpdateUserDelegate += this.Zig_UpdateUser;
	}
	
	// Update is called once per frame
	public override void Update () {
		//TODO take points project on plane and draw on screen
//mZigObject.GetComponent<kinectSpecific>().enabled = false;
           
	}

    void set_relative_position(ZigInputJoint aJoint, Vector3 aCenter)
    {
        Vector3 rp = aJoint.Position - aCenter;
        //Debug.Log("setting joint " + aJoint.Id.ToString());
        //TODO
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
                mJoints[joint.Id] = joint;
                if(joint.GoodPosition && joint.GoodRotation)
                {
                    set_relative_position(joint,user.Position);
                }
            }
        }
    } 
}
