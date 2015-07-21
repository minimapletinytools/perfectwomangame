using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PhysicsFlatBodyObject 
{
	
	FlatBodyObject mFlat;
	//GameObject mPhysBodyParent = null;
	//these are all the rigid bodies
	Dictionary<ZgJointId,GameObject> mBodies = new Dictionary<ZgJointId, GameObject>();
	//these are the joint offsets (because hinge joints starting angle is relative)
	public struct JointOffset{public HingeJoint joint; public Quaternion offset;}
	Dictionary<ZgJointId,JointOffset> mJointAngleOffset = new Dictionary<ZgJointId, JointOffset>();
	
	public class Stupid
    {
        public List<ZgJointId> otherEnds = new List<ZgJointId>();
        public Stupid(ZgJointId other) { otherEnds.Add(other); }
		public Stupid(ZgJointId[] other) { otherEnds.AddRange(other); }
		public Stupid(){}
    }

	//key is body, value is list of things attached to body
    public Dictionary<ZgJointId, Stupid> mImportant = new Dictionary<ZgJointId, Stupid>();
	
	public PhysicsFlatBodyObject(FlatBodyObject aObject)
	{
		mFlat = aObject;

		mImportant[ZgJointId.Torso] = new Stupid(new ZgJointId[]{ZgJointId.LeftShoulder,ZgJointId.RightShoulder,ZgJointId.Waist,ZgJointId.Neck});
		mImportant[ZgJointId.Waist] = new Stupid(new ZgJointId[]{ZgJointId.LeftHip,ZgJointId.RightHip});
		mImportant[ZgJointId.LeftShoulder] = new Stupid(ZgJointId.LeftElbow);
        mImportant[ZgJointId.LeftElbow] = new Stupid(ZgJointId.LeftHand);
        mImportant[ZgJointId.LeftHip] = new Stupid(ZgJointId.LeftKnee);
        mImportant[ZgJointId.LeftKnee] = new Stupid(ZgJointId.LeftAnkle);
        mImportant[ZgJointId.RightShoulder] = new Stupid(ZgJointId.RightElbow);
        mImportant[ZgJointId.RightElbow] = new Stupid(ZgJointId.RightHand);
        mImportant[ZgJointId.RightHip] = new Stupid(ZgJointId.RightKnee);
        mImportant[ZgJointId.RightKnee] = new Stupid(ZgJointId.RightAnkle);
        mImportant[ZgJointId.Neck] = new Stupid(ZgJointId.Head);
		mImportant[ZgJointId.Head] = new Stupid();
		mImportant[ZgJointId.LeftHand] = new Stupid();
		mImportant[ZgJointId.RightHand] = new Stupid();
		mImportant[ZgJointId.LeftAnkle] = new Stupid();
		mImportant[ZgJointId.RightAnkle] = new Stupid();

	}
	
	public Vector3 mNormal = Vector3.forward;
	public Vector3 mUp = Vector3.up;
	public float get_relative(Vector3 A, Vector3 B)
    {
        Vector3 right = Vector3.Cross(mUp, mNormal);
        Vector3 v = B - A;
       
        Vector3 projected = Vector3.ProjectOnPlane(mNormal, v);
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
		foreach (KeyValuePair<ZgJointId, ProjectionManager.Stupid> e in aManager.mImportant)
		{
			//the torso will be skipped because in physics, the torse angle is determined by the waist
			if(mJointAngleOffset.ContainsKey(e.Key)){
				set_hinge_position(-mJointAngleOffset[e.Key].offset.eulerAngles.z + e.Value.smoothing.current,mJointAngleOffset[e.Key].joint);
			}
		}
		set_hinge_position(-mJointAngleOffset[ZgJointId.Waist].offset.eulerAngles.z + aManager.mWaist.current,mJointAngleOffset[ZgJointId.Waist].joint);


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
			main.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationY;
			main.GetComponent<Rigidbody>().drag = 1;
			main.GetComponent<Rigidbody>().angularDrag = 1;
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
					joint.connectedBody = mBodies[e.Key].GetComponent<Rigidbody>();
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

