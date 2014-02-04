using UnityEngine;
using System.Collections.Generic;

public class PhysicsFlatBodyObject 
{
	
	FlatBodyObject mFlat;
	GameObject mPhysBodyParent = null;
	Dictionary<ZigJointId,GameObject> mColliders = new Dictionary<ZigJointId, GameObject>();
	
	public class Stupid
    {
        public ZigJointId otherEnd;
        public Stupid(ZigJointId other) { otherEnd = other; }
    }
    public Dictionary<ZigJointId, Stupid> mImportant = new Dictionary<ZigJointId, Stupid>();
	
	public PhysicsFlatBodyObject(FlatBodyObject aObject)
	{
		mFlat = aObject;
		
		
		mImportant[ZigJointId.LeftShoulder] = new Stupid(ZigJointId.LeftElbow);
        mImportant[ZigJointId.LeftElbow] = new Stupid(ZigJointId.LeftHand);
        mImportant[ZigJointId.LeftHip] = new Stupid(ZigJointId.LeftKnee);
        mImportant[ZigJointId.LeftKnee] = new Stupid(ZigJointId.LeftAnkle);
        mImportant[ZigJointId.RightShoulder] = new Stupid(ZigJointId.RightElbow);
        mImportant[ZigJointId.RightElbow] = new Stupid(ZigJointId.RightHand);
        mImportant[ZigJointId.RightHip] = new Stupid(ZigJointId.RightKnee);
        mImportant[ZigJointId.RightKnee] = new Stupid(ZigJointId.RightAnkle);
        mImportant[ZigJointId.Neck] = new Stupid(ZigJointId.Head);
        mImportant[ZigJointId.Torso] = new Stupid(ZigJointId.Neck);
	}
	
	public void set_target_pose(Pose aPose, bool hard = false)
    {
        mFlat.set_target_pose(aPose, hard);
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
	
	public void update()
	{
		//force to stay in 2d plane
		mPhysBodyParent.transform.eulerAngles =  new Vector3(0,0,mPhysBodyParent.transform.eulerAngles.z);
		mPhysBodyParent.transform.position = new Vector3(mPhysBodyParent.transform.position.x,mPhysBodyParent.transform.position.y,0);
		mColliders[ZigJointId.Torso].transform.eulerAngles = new Vector3(0,0,mColliders[ZigJointId.Torso].transform.eulerAngles.z);
		mColliders[ZigJointId.Torso].transform.position = new Vector3(mColliders[ZigJointId.Torso].transform.position.x,mColliders[ZigJointId.Torso].transform.position.y,0);
		
		//apply gravity to body first
		mFlat.HardPosition = mColliders[ZigJointId.Torso].transform.position;
		mFlat.HardFlatRotation = mColliders[ZigJointId.Torso].transform.rotation.flat_rotation();
		mFlat.update_parameters(Time.deltaTime);
		mFlat.set();
		
		
		//try to move physics body to mFlat to new positions
		foreach(var e in mFlat.mParts)
		{
			mColliders[e.Key].rigidbody.MovePosition(e.Value.transform.position);
			//mColliders[e.Key].transform.position = (e.Value.transform.position);
		}	
		
		
		
		//update mflat with physics body info (visuals)
		Pose np = mFlat.get_pose();
		foreach(var e in np.mElements)
		{
			if(mImportant.ContainsKey(e.joint) && (mImportant[e.joint].otherEnd != ZigJointId.Head))
			{
				//Debug.Log("trying for " + e.joint + " with " + mImportant[e.joint].otherEnd);
				e.angle = get_relative(mColliders[e.joint].transform.position,mColliders[mImportant[e.joint].otherEnd].transform.position);
			}
		}
		mFlat.set_target_pose(np,true);
		
		//mFlat.HardPosition = mPhysBodyParent.transform.position;
		//mFlat.HardFlatRotation = mPhysBodyParent.transform.rotation.flat_rotation();
		mFlat.HardPosition = mColliders[ZigJointId.Torso].transform.position;
		mFlat.HardFlatRotation = mColliders[ZigJointId.Torso].transform.rotation.flat_rotation();
		mFlat.update_parameters(Time.deltaTime);
		mFlat.set();
			
		//force to stay in 
		//mPhysBodyParent.rigidbody.MoveRotation(new Vector3(0,0,mPhysBodyParent.transform.eulerAngles.z));
		//mPhysBodyParent.transfo
	}
	
	public Pose physics_pose()
	{
		return null;	
	}
	
	public void setup_body_with_physics()
	{
		
		mPhysBodyParent = new GameObject("genPhysBodyParent");
		mPhysBodyParent.AddComponent<Rigidbody>();
		
		foreach (KeyValuePair<ZigJointId, GameObject> e in mFlat.mParts)
        {
			GameObject endpoint = null;
			for(int i =0; i < e.Value.transform.childCount; i++)
			{
				if(e.Value.transform.GetChild(i).name.Contains("gen"))
				{
					endpoint = e.Value.transform.GetChild(i).gameObject;
					break;
				}
			}
			
			
			//we do it the ghetto way and only use sphere collidres
	
			mColliders[e.Key] = new GameObject("genCollider"+e.Key.ToString());
			var col = mColliders[e.Key].AddComponent<SphereCollider>();
			col.radius = 50;
			mColliders[e.Key].transform.position = e.Value.transform.position;
			mColliders[e.Key].AddComponent<Rigidbody>();
			mColliders[e.Key].layer =  1 << 3;
			//add_fixed_joint(mColliders[e.Key]);
			
			//var configJoint = mColliders[e.Key].AddComponent<
			
			//mColliders[e.Key].transform.parent = mPhysBodyParent;
		}
	}
	
	
	public void add_fixed_joint(GameObject obj)
	{
		var joint = obj.AddComponent<FixedJoint>();
			joint.connectedBody = mPhysBodyParent.rigidbody;
			joint.breakForce = Mathf.Infinity;
			joint.breakTorque = Mathf.Infinity;
	}
}

