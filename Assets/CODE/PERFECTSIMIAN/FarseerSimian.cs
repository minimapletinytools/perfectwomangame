using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

public static class FarseerExt
{
	public static FVector2 toFV2(this Vector2 vec)
	{
		return new FVector2(vec.x,vec.y);
	}
	public static FVector2 toFV2(this Vector3 vec)
	{
		return new FVector2(vec.x,vec.y);
	}
	public static Vector3 toV3(this FVector2 vec)
	{
		return new Vector3(vec.X,vec.Y,0);
	}
}

public class FarseerSimian 
{
	public class BodyGroup
	{
		public FarseerPhysics.Dynamics.Body body;
		public float offset;
		public float relJointOffset;
		public FarseerPhysics.Dynamics.Joints.RevoluteJoint joint;
	}
	Dictionary<ZigJointId,BodyGroup> mBodies = new Dictionary<ZigJointId, BodyGroup>();
	FlatBodyObject mFlat;
	FSWorldComponent mWorld;

	public void add_environment(IEnumerable<GameObject> aObjects)
	{
		foreach(var e in aObjects)
		{
			Debug.Log ("added static elt " + e.name);
			e.AddComponent<FSShapeComponent>().UseUnityCollider = true;
			var bd = e.AddComponent<FSBodyComponent>();
			bd.Type = FarseerPhysics.Dynamics.BodyType.Static;
		}
	}

	public void initialize(GameObject useMe)
	{
		//hardcode in the desired joints
		mImportant[ZigJointId.Torso] = new Stupid(new ZigJointId[]{ZigJointId.LeftShoulder,ZigJointId.Neck,ZigJointId.RightShoulder,ZigJointId.Waist});
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

		mWorld = useMe.AddComponent<FSWorldComponent>();
		var debug = useMe.AddComponent<FSDebugDrawComponent>();
		debug.GLMaterial = useMe.GetComponent<NewMenuReferenceBehaviour>().farseerMaterial;
	}

	public void setup_with_body(FlatBodyObject aBody)
	{
		mFlat = aBody;
		//construct the bodies
		foreach(var e in mImportant)
		{
			//note this will cerate 'dummy bodies' representing hand/ankle/head
			BodyGroup bg = new BodyGroup();
			bg.offset = aBody.mParts[e.Key].transform.rotation.flat_rotation();
			bg.body = BodyFactory.CreateBody(FSWorldComponent.PhysicsWorld,mFlat.mParts[e.Key].transform.position.toFV2());
			bg.body.Mass = 1;
			bg.body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
			mBodies[e.Key] = bg;
			new GameObject(e.Key.ToString()).transform.position = mFlat.mParts[e.Key].transform.position;

		}

		//now create fixtures
		foreach(var e in mImportant)
		{

			if(e.Value.otherEnds.Count > 0)
			{
				List<FarseerPhysics.Common.Vertices> poly = new List<FarseerPhysics.Common.Vertices>();
				poly.Add(new FarseerPhysics.Common.Vertices());
				if(e.Key != ZigJointId.Torso) //torso does not need this point, only exception
					poly[0].Add(FVector2.Zero);
				if(e.Value.otherEnds.Count == 1)
				{
					//TODO add girth???
					//TODO rotation????
					foreach(var f in e.Value.otherEnds)
					{
						poly[0].Add(mFlat.mParts[f].transform.position.toFV2()-mBodies[e.Key].body.Position);
					}
				}
				else
				{
					foreach(var f in e.Value.otherEnds)
					{
						poly[0].Add(mFlat.mParts[f].transform.position.toFV2()-mBodies[e.Key].body.Position);
					}
				}
				FixtureFactory.AttachCompoundPolygon(poly,1,mBodies[e.Key].body);
			}
		}
		
		//create joints
		foreach (var e in mImportant)
		{
			foreach(var f in e.Value.otherEnds)
			{
				if(mImportant[f].otherEnds.Count > 0)
				{
					var joint = JointFactory.CreateRevoluteJoint(FSWorldComponent.PhysicsWorld,mBodies[e.Key].body,mBodies[f].body,FVector2.Zero);
					joint.CollideConnected = false;
					//assosciate the joint with the childed limb
					mBodies[f].joint = joint;
					//mBodies[f].relJointOffset = get_relative(mImportant
				}
			}
		}
	}

	public void update(ProjectionManager aManager)
	{
		//TODO
		//set desired position from projection manager
		foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in aManager.mImportant)
		{
			if(mBodies.ContainsKey(e.Key))
			{
				//mBodies[e.Key].joint.
				//set desired value to this (we copmutre everything rel. to torseo)
				//e.Value.smoothing.current-aManager.mImportant[ZigJointId.Torso].smoothing.current;
			}
			//the torso will be skipped because in physics, the torse angle is determined by the waist
			//if(mJointAngleOffset.ContainsKey(e.Key)){
				//set_hinge_position(-mJointAngleOffset[e.Key].offset.eulerAngles.z + e.Value.smoothing.current,mJointAngleOffset[e.Key].joint);
			//}
		}
		//set_hinge_position(-mJointAngleOffset[ZigJointId.Waist].offset.eulerAngles.z + aManager.mWaist.current,mJointAngleOffset[ZigJointId.Waist].joint);

		//update flat to be what you see

		mFlat.set_target_pose(physics_pose());
		mFlat.SoftPosition = mBodies[ZigJointId.Torso].body.Position.toV3();
		//mFlat.HardPosition = mFlat.SoftPosition;
		mFlat.update(Time.deltaTime);
		mFlat.set();
	}

	public Pose physics_pose()
	{
		Pose r = new Pose();
		foreach(var e in mBodies)
		{
			PoseElement pe = new PoseElement();
			pe.joint = e.Key;
			pe.angle = e.Value.offset + e.Value.body.Rotation/Mathf.PI*180;
			r.mElements.Add(pe);
		}
		return r;
	}


	//stupid stuff needed for construction
	public Dictionary<ZigJointId, Stupid> mImportant = new Dictionary<ZigJointId, Stupid>();
	public class Stupid
	{
		public List<ZigJointId> otherEnds = new List<ZigJointId>();
		public Stupid(ZigJointId other) { otherEnds.Add(other); }
		public Stupid(ZigJointId[] other) { otherEnds.AddRange(other); }
		public Stupid(){}
	}


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
}
