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
		mImportant[ZigJointId.Waist] = new Stupid(new ZigJointId[]{ZigJointId.RightHip,ZigJointId.LeftHip});
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
				if(e.Key != ZigJointId.Torso || e.Value.otherEnds.Count == 1) //torso does not need this point
					poly[0].Add(FVector2.Zero);
				if(e.Value.otherEnds.Count == 1)
				{
					//line version
					//foreach(var f in e.Value.otherEnds)
					//	poly[0].Add(mFlat.mParts[f].transform.position.toFV2()-mBodies[e.Key].body.Position);

					//block version
					FVector2 diff = mFlat.mParts[e.Value.otherEnds[0]].transform.position.toFV2()-mBodies[e.Key].body.Position;
					FVector2 perp = new FVector2(diff.Y,-diff.X);
					perp.Normalize();
					float stretch = .03f;
					poly[0].Add(perp*stretch);
					poly[0].Add(diff + perp*stretch);
					poly[0].Add(diff - perp*stretch);
					poly[0].Add(-perp*stretch);
				}
				else
				{
					foreach(var f in e.Value.otherEnds)
					{
						poly[0].Add(mFlat.mParts[f].transform.position.toFV2()-mBodies[e.Key].body.Position);
					}
				}
				var fixture = FixtureFactory.AttachCompoundPolygon(poly,1,mBodies[e.Key].body);
				fixture[0].CollisionGroup = 1;
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
					mBodies[f].joint = joint;
				}
			}
		}

		//clean up the dummy bodies
		foreach (var e in mImportant) {
			if (e.Value.otherEnds.Count == 0) {
				FSWorldComponent.PhysicsWorld.RemoveBody(mBodies[e.Key].body);
				mBodies.Remove(e.Key);
			}
		}
	}

	public void update(ProjectionManager aManager)
	{
		//set desired position from projection manager
		foreach (KeyValuePair<ZigJointId, ProjectionManager.Stupid> e in aManager.mImportant)
		{
			if(mBodies.ContainsKey(e.Key))
			{
				//ManagerManager.Manager.mDebugString = mBodies[ZigJointId.Waist].body.Rotation.ToString();
				if(e.Key != ZigJointId.Waist)
				{
					//rotate_body_to(mBodies[e.Key].body,mBodies[ZigJointId.Waist].body.Rotation + (e.Value.smoothing.current-mBodies[e.Key].offset) * Mathf.PI/180f);
				}
			}
		}

		//update the visual body to match the physics body
		mFlat.set_target_pose(physics_pose());
		mFlat.SoftInterpolation = .5f;
		mFlat.SoftPosition = mBodies[ZigJointId.Torso].body.Position.toV3()/GameConstants.SCALE;
		//mFlat.HardPosition = mFlat.SoftPosition;
		mFlat.update (0);
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

	void rotate_body_to(FarseerPhysics.Dynamics.Body body, float desiredAngle)
	{
		float DEGTORAD = Mathf.PI/180f;
		float bodyAngle = body.Rotation;
		float nextAngle = bodyAngle + body.AngularVelocity * Time.deltaTime;
		float totalRotation = desiredAngle - nextAngle;
		while ( totalRotation < -180 * DEGTORAD ) totalRotation += 360 * DEGTORAD;
		while ( totalRotation >  180 * DEGTORAD ) totalRotation -= 360 * DEGTORAD;
		float desiredAngularVelocity = totalRotation * 60;
		float impulse = body.Inertia * desiredAngularVelocity;// disregard time factor
		body.ApplyAngularImpulse( impulse/3f );
	}
			
}
