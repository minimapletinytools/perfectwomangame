using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}

    Dictionary<ZigJointId, GameObject> mParts = new Dictionary<ZigJointId, GameObject>();
	
	public float convert_units(double pixelWidth)
	{
		return (float)pixelWidth/100.0f; //100 pixels = 1 unit
	}
	public GameObject create_object(ZigJointId aId, Texture2D aTex)
	{
		GameObject parent = new GameObject("genParent"+aId.ToString());
		GameObject kid = GameObject.CreatePrimitive(PrimitiveType.Plane);
		kid.transform.localScale = new Vector3(convert_units(aTex.width)/10.0f,1,convert_units(aTex.height)/10.0f);
		kid.transform.rotation = Quaternion.AngleAxis(90,Vector3.forward)*Quaternion.AngleAxis(90,Vector3.right)*Quaternion.AngleAxis(90,Vector3.up)*kid.transform.rotation;
        //TODO kid.transform.position = get_connection_point
		kid.transform.parent = parent.transform;
		mParts[aId] = parent;
		return parent;
	}
	public Vector3 get_connection_point(ZigJointId A, ZigJointId B, Texture2D aBTex) //returns connection point of A to B
	{
		if(B == ZigJointId.Waist)
		{
			if(A == ZigJointId.Torso)
				return new Vector3(0,convert_units(aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.LeftHip)
				return new Vector3(convert_units(-aBTex.width/2.0*0.8),convert_units(-aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.RightHip)
				return new Vector3(convert_units(aBTex.width/2.0*0.8),convert_units(-aBTex.height/2.0*0.8),0);
		}
		if(B == ZigJointId.Torso)
		{
			if(A == ZigJointId.Neck)
				return new Vector3(0,convert_units(aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.LeftShoulder)
				return new Vector3(convert_units(-aBTex.width/2.0*0.8),convert_units(aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.RightShoulder)
				return new Vector3(convert_units(aBTex.width/2.0*0.8),convert_units(aBTex.height/2.0*0.8),0);
		}
		else if(A == ZigJointId.Torso)
		{
			return new Vector3(0,0,0); //TODO
		}
		else if(A != ZigJointId.None)
		{
			return new Vector3(0,convert_units(aBTex.height/2.0 - aBTex.width*0.1),0);
		}
		else
		{
			return Vector3.zero; //TODO this should be where the character is maybe?
		}
		
		return Vector3.zero;
		/*
		else if(A == ZigJointId.LeftShoulder)
		{	
		}
		else if(A == ZigJointId.RightShoulder)
		{
		}
		else if(A == ZigJointId.LeftElbow)
		{
		}
		else if(A == ZigJointId.RightElbow)
		{
		}
		else if(A == ZigJointId.LeftHip)
		{
		}
		else if(A == ZigJointId.RightHip)
		{
		}
		else if(A == ZigJointId.LeftKnee)
		{
		}
		else if(A == ZigJointId.RightKnee)
		{
		}*/
	}
	public override void Start () {
        
		GameObject demoChar = (GameObject)GameObject.Instantiate(mManager.mReferences.mDemoChar);
		create_body(demoChar.GetComponent<CharacterTextureBehaviour>());
	}

    public override void Update()
    {
        
		foreach(KeyValuePair<GradingManager.WeightedZigJointPair,ProjectionManager.Smoothing> e in mManager.mProjectionManager.mImportant)
		{
            if (e.Key.A == ZigJointId.None)
            {
                //TODO
            }
            else
            {
                mParts[e.Key.A].transform.localRotation = Quaternion.AngleAxis(e.Value.current, Vector3.forward);
            }
		}
	}
	
	public void create_body(CharacterTextureBehaviour aChar)
	{
		GameObject torso = create_object(ZigJointId.Torso,aChar.torso);
		GameObject waist = create_object(ZigJointId.Waist,aChar.waist);
		GameObject head = create_object(ZigJointId.Neck,aChar.head);
		GameObject leftUpperArm = create_object(ZigJointId.LeftShoulder,aChar.leftUpperArm);
		GameObject rightUpperArm = create_object(ZigJointId.RightShoulder,aChar.rightUpperArm);
		GameObject leftLowerArm = create_object(ZigJointId.LeftElbow,aChar.leftLowerArm);
		GameObject rightLowerArm = create_object(ZigJointId.RightElbow,aChar.rightLowerArm);
		GameObject leftUpperLeg = create_object(ZigJointId.LeftHip,aChar.leftUpperLeg);
		GameObject rightUpperLeg = create_object(ZigJointId.RightHip,aChar.rightUpperLeg);
		GameObject leftLowerLeg = create_object(ZigJointId.LeftKnee,aChar.leftLowerLeg);
		GameObject rightLowerLeg = create_object(ZigJointId.RightKnee,aChar.rightLowerLeg);
		
        //TODO connect waist somewhere??
		torso.transform.position = get_connection_point(ZigJointId.Torso,ZigJointId.Waist,aChar.waist);
		torso.transform.parent = waist.transform;//TODO DELEE we don't need this anymore
		head.transform.position = get_connection_point(ZigJointId.Neck,ZigJointId.Torso,aChar.torso);
		head.transform.parent = torso.transform;
		leftUpperArm.transform.position = get_connection_point(ZigJointId.LeftShoulder,ZigJointId.Torso,aChar.torso);
		leftUpperArm.transform.parent = torso.transform;
		rightUpperArm.transform.position = get_connection_point(ZigJointId.RightShoulder,ZigJointId.Torso,aChar.torso);
		rightUpperArm.transform.parent = torso.transform;
		leftLowerArm.transform.position = get_connection_point(ZigJointId.LeftElbow,ZigJointId.LeftShoulder,aChar.leftUpperArm);
		leftLowerArm.transform.parent = leftUpperArm.transform;
		rightLowerArm.transform.position = get_connection_point(ZigJointId.RightElbow,ZigJointId.RightShoulder,aChar.rightUpperArm);
		rightLowerArm.transform.parent = rightUpperArm.transform;
		
		leftUpperLeg.transform.position = get_connection_point(ZigJointId.LeftHip,ZigJointId.Waist,aChar.torso);
		leftUpperLeg.transform.parent = torso.transform;
		rightUpperLeg.transform.position = get_connection_point(ZigJointId.RightHip,ZigJointId.Waist,aChar.torso);
		rightUpperLeg.transform.parent = torso.transform;
		leftLowerLeg.transform.position = get_connection_point(ZigJointId.LeftElbow,ZigJointId.LeftHip,aChar.leftUpperLeg);
		leftLowerLeg.transform.parent = leftUpperLeg.transform;
		rightLowerLeg.transform.position = get_connection_point(ZigJointId.RightElbow,ZigJointId.RightHip,aChar.rightUpperLeg);
		rightLowerLeg.transform.parent = rightUpperLeg.transform;
		
		
	}
}
