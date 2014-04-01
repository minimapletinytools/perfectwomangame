using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PhysicsFlatBodyObject 
{
	
	FlatBodyObject mFlat;
	GameObject mPhysBodyParent = null;
	//these are all the rigid bodies
	Dictionary<ZigJointId,GameObject> mBodies = new Dictionary<ZigJointId, GameObject>();
	//these are the joint offsets (because hinge joints starting angle is relative)
	public struct JointOffset{public HingeJoint joint; public Quaternion offset;}
	Dictionary<ZigJointId,JointOffset> mJointAngleOffset = new Dictionary<ZigJointId, JointOffset>();
	
	public class Stupid
    {
        public List<ZigJointId> otherEnds = new List<ZigJointId>();
        public Stupid(ZigJointId other) { otherEnds.Add(other); }
		public Stupid(ZigJointId[] other) { otherEnds.AddRange(other); }
		public Stupid(){}
    }

	//key is body, value is list of things attached to body
    public Dictionary<ZigJointId, Stupid> mImportant = new Dictionary<ZigJointId, Stupid>();
	
	public PhysicsFlatBodyObject(FlatBodyObject aObject)
	{
		mFlat = aObject;

		mImportant[ZigJointId.Torso] = new Stupid(new ZigJointId[]{ZigJointId.LeftShoulder,ZigJointId.RightShoulder,ZigJointId.Waist,ZigJointId.Neck});
		mImportant[ZigJointId.Waist] = new Stupid(new ZigJointId[]{ZigJointId.LeftHip,ZigJointId.RightHip});
		mImportant[ZigJointId.LeftShoulder] = new Stupid(ZigJointId.LeftElbow);
        mImportant[ZigJointId.LeftElbow] = new Stupid(ZigJointId.LeftHand);
        mImportant[ZigJointId.LeftHip] = new Stupid(ZigJointId.LeftKnee);
        mImportant[ZigJointId.LeftKnee] = new Stupid(ZigJointId.LeftAnkle);
        mImportant[ZigJointId.RightShoulder] = new Stupid(ZigJointId.RightElbow);
        mImportant[ZigJointId.RightElbow] = new Stupid(ZigJointId.RightHand);
        mImportant[ZigJointId.RightHip] = new Stupid(ZigJointId.RightKnee);
        mImportant[ZigJointId.RightKnee] = new Stupid(ZigJointId.RightAnkle);
        mImportant[ZigJointId.Neck] = new Stupid(ZigJointId.Head);
		mImportant[ZigJointId.Head] = new Stupid();
		mImportant[ZigJointId.LeftHand] = new Stupid();
		mImportant[ZigJointId.RightHand] = new Stupid();
		mImportant[ZigJointId.LeftAnkle] = new Stupid();
		mImportant[ZigJointId.RightAnkle] = new Stupid();

	}
	
	public Vector3 mNormal = Vector3.forward;
	public Vector3 mUp = Vector3.up;
	public float get_relative(Vector3 A, Vector3 B)
    {
        Vector3 right = Vector3.Cross(mUp, mNormal);
        Vector3 v = B - A;
       
        Vector3 projected = Vector3.Exclude(mNormal, v);
        float r = Vector3.Angle(right, projected);
        if (Vector3.Dot(Vector3.Cross(right, projected), mNormal) < 0)
        {
            r *= -1;
        }
        return -r;
    }
	
	public void update(ProjectionManager aManager)
	{

		//set desired position from projection manager
		foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in aManager.mImportant)
		{
			//the torso will be skipped because in physics, the torse angle is determined by the waist
			if(mJointAngleOffset.ContainsKey(e.Key)){
				set_hinge_position(-mJointAngleOffset[e.Key].offset.eulerAngles.z + e.Value.smoothing.current,mJointAngleOffset[e.Key].joint);
			}
		}
		set_hinge_position(-mJointAngleOffset[ZigJointId.Waist].offset.eulerAngles.z + aManager.mWaist.current,mJointAngleOffset[ZigJointId.Waist].joint);


		foreach(var e in mFlat.mParts)
		{
			Debug.Log (e.Key);
			e.Value.transform.position = mBodies[e.Key].transform.position;
			e.Value.transform.rotation = mBodies[e.Key].transform.rotation ;
		}

	}
	
	public Pose physics_pose()
	{
		return null;	
	}
	
	public void setup_body_with_physics()
	{
		//construct rigid bodies and colliders
		foreach(var e in mImportant)
		{
			GameObject main = new GameObject("gen"+e.Key.ToString());
			main.transform.position = mFlat.mParts[e.Key].transform.position;
			main.AddComponent<Rigidbody>();
			main.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationY;
			main.rigidbody.drag = 1;
			main.rigidbody.angularDrag = 1;
			//main.rigidbody.useGravity = false;
			mBodies[e.Key] = main;
			foreach(var f in e.Value.otherEnds)
			{
				GameObject colliderObject = new GameObject("genCollider"+f.ToString());
				colliderObject.transform.position = mFlat.mParts[f].transform.position;
				colliderObject.AddComponent<SphereCollider>().radius = 30;
				colliderObject.transform.parent = main.transform;
				colliderObject.layer =  1 << 3;
			}
		}

		//create joints
		foreach (var e in mImportant)
		{
			foreach(var f in e.Value.otherEnds)
			{
				if(mImportant.ContainsKey(f))
				{
					//create a joint anchored at f between f and e
					var joint = mBodies[f].AddComponent<HingeJoint>();
					joint.anchor = Vector3.zero;
					joint.connectedBody = mBodies[e.Key].rigidbody;
					mJointAngleOffset[f] = new JointOffset{joint = joint,offset = mFlat.mParts[f].transform.rotation}; //this should be ok...
				}
			}
		}
	}

	public void set_hinge_position(float position, HingeJoint joint)
	{
		var lim = joint.limits;
		lim.min = lim.max = 0;//lim.min*0.95f + 0.05f*position;
		joint.limits = lim;
	}
}

