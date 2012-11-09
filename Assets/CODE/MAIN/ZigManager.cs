using UnityEngine;
using System.Collections;

public class ZigManager : FakeMonoBehaviour {
	GameObject mZigObject = null;
	Zig mZig = null;
	ZigEngageSingleUser mZigEngageSingleUser = null;
    public ZigManager(ManagerManager aManager) : base(aManager)
	{
	}

	// Use this for initialization
	public override void Start () {
		mZigObject = new GameObject("genZig");
		mZig = mZigObject.AddComponent<Zig>();
		mZig.listeners.Add(mZigObject);
		mZigEngageSingleUser = mZigObject.AddComponent<ZigEngageSingleUser>();
		mZigEngageSingleUser.EngagedUsers = new System.Collections.Generic.List<UnityEngine.GameObject>();
		mZigEngageSingleUser.EngagedUsers.Add(mZigObject);
	}
	
	// Update is called once per frame
	public override void Update () {
		//TODO take points project on plane and draw on screen
		
	}
	
	
	void Zig_UpdateUser(ZigTrackedUser user)
    {
		//TODO lol
		/*
        string o = "";
        UpdateRoot(user.Position);
        if (user.SkeletonTracked)
        {
            foreach (ZigInputJoint joint in user.Skeleton)
            {
                if(joint.GoodPosition)
                    o += " " + joint.Id.ToString();
                if (joint.Id == ZigJointId.LeftFingertip)
                    Debug.Log(joint.Position);
                
                if (joint.GoodPosition) UpdatePosition(joint.Id, joint.Position);
                if (joint.GoodRotation) UpdateRotation(joint.Id, joint.Rotation);
            }
        }
        */
        //Debug.Log(o);
    }
}
