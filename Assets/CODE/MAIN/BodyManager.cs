using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}

    Dictionary<ZigJointId, GameObject> mParts = new Dictionary<ZigJointId, GameObject>();

    GameObject get_limb_top(GameObject o)
    {
        return o.transform.FindChild("TOP").gameObject;
    }
    GameObject get_limb_bottom(GameObject o)
    {
        return o.transform.FindChild("BOTTOM").gameObject;
    }
	public override void Start () {
        
        mParts[ZigJointId.LeftShoulder] = (GameObject)GameObject.Instantiate(mManager.mReferences.mLimbObject);
        mParts[ZigJointId.LeftElbow] = (GameObject)GameObject.Instantiate(mManager.mReferences.mLimbObject, get_limb_bottom(mParts[ZigJointId.LeftShoulder]).transform.position,Quaternion.identity);
        mParts[ZigJointId.LeftElbow].transform.parent = get_limb_bottom(mParts[ZigJointId.LeftShoulder]).transform;
        mParts[ZigJointId.LeftHand] = (GameObject)GameObject.Instantiate(mManager.mReferences.mLimbObject, get_limb_bottom(mParts[ZigJointId.LeftElbow]).transform.position, Quaternion.identity);
        mParts[ZigJointId.LeftHand].transform.parent = get_limb_bottom(mParts[ZigJointId.LeftElbow]).transform;
	}

    public override void Update()
    {
        foreach(KeyValuePair<ZigJointId,GameObject> e in mParts)
        {
            if (mManager.mZigManager.mJoints.ContainsKey(e.Key))
            {
                Debug.Log("Rotating");
                mParts[e.Key].transform.rotation = mManager.mZigManager.mJoints[e.Key].Rotation;
            }
        }
	}
}
