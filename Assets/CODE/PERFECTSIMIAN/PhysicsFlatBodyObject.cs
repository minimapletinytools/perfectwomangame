using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PhysicsFlatBodyObject 
{
	
	FlatBodyObject mFlat;
	GameObject mPhysBodyParent = null;
	Dictionary<ZigJointId,GameObject> mColliders = new Dictionary<ZigJointId, GameObject>();
	
	public class Stupid
    {
        public List<ZigJointId> otherEnds = new List<ZigJointId>();
        public Stupid(ZigJointId other) { otherEnds.Add(other); }
		public Stupid(ZigJointId[] other) { otherEnds.AddRange(other); }
    }

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
			//TODO
			//if(mImportant.ContainsKey(e.joint) && (mImportant[e.joint].otherEnd != ZigJointId.Head))
			{
				//Debug.Log("trying for " + e.joint + " with " + mImportant[e.joint].otherEnd);
				//e.angle = get_relative(mColliders[e.joint].transform.position,mColliders[mImportant[e.joint].otherEnd].transform.position);
				//TODO
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
		
		foreach (KeyValuePair<ZigJointId, GameObject> e in mFlat.mParts)
        {
			//compute the endpoint
			GameObject endpoint = null;
			for(int i =0; i < e.Value.transform.childCount; i++)
			{
				if(e.Value.transform.GetChild(i).name.Contains("gen"))
				{
					endpoint = e.Value.transform.GetChild(i).gameObject;
					break;
				}
			}
	
			//add a collider
			mColliders[e.Key] = new GameObject("genCollider"+e.Key.ToString());
			var col = mColliders[e.Key].AddComponent<SphereCollider>();
			col.radius = 50;
			mColliders[e.Key].transform.position = e.Value.transform.position;
			mColliders[e.Key].AddComponent<Rigidbody>();
			mColliders[e.Key].layer =  1 << 3;


		}

		//create parent attachements and add rigid bodies
		foreach (ZigJointId e in mImportant.Keys)
		{
			foreach(ZigJointId f in mImportant[e].otherEnds)
				mColliders[f].transform.parent = mColliders[e].transform;
			var rb = mColliders[e].AddComponent<Rigidbody>();
			//TODO setup rb
		}
		foreach (var e in mImportant.Keys)
		{
			//figure out what its attached to
			var otherEnd = mImportant.First(f=>f.Value.otherEnds.Contains(e));

			//now we want a hinge joint between otherend, e.key anchored at e.key
			Vector3 anchor = Vector3.zero; //TODO
			create_hinge_joint(mColliders[e].gameObject,mColliders[otherEnd.Key],anchor);
		}
	}
	
	
	public void add_fixed_joint(GameObject obj)
	{
		var joint = obj.AddComponent<FixedJoint>();
			joint.connectedBody = mPhysBodyParent.rigidbody;
			joint.breakForce = Mathf.Infinity;
			joint.breakTorque = Mathf.Infinity;
	}

	public void create_hinge_joint(GameObject obj1, GameObject obj2, Vector3 anchor)
	{
		var joint = obj1.AddComponent<HingeJoint>();
		joint.connectedBody = obj2.rigidbody; //obj2 will always have a rigidbody 
		joint.axis = Vector3.forward;
		joint.anchor = anchor;
	}
}

