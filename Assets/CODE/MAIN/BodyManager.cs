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
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.localScale = Vector3.one * 0.2f;
		sphere.transform.parent = parent.transform;
		GameObject kid = GameObject.CreatePrimitive(PrimitiveType.Plane);
        kid.transform.rotation =  Quaternion.AngleAxis(90, Vector3.right) * kid.transform.rotation;
        kid.transform.localScale = new Vector3(convert_units(aTex.width) / 10.0f, 1, convert_units(aTex.height) / 10.0f);
        if (aId == ZigJointId.Neck || aId == ZigJointId.Torso)
            kid.transform.position = new Vector3(0, convert_units(aTex.height / 2.0f * 0.8f), 0);
        if (aId == ZigJointId.LeftShoulder || aId == ZigJointId.RightShoulder || aId == ZigJointId.LeftHip || aId == ZigJointId.RightHip || 
            aId == ZigJointId.LeftElbow || aId == ZigJointId.RightElbow || aId == ZigJointId.LeftKnee || aId == ZigJointId.RightKnee || aId == ZigJointId.Waist)
            kid.transform.position = new Vector3(0, convert_units(-aTex.height / 2.0f * 0.8f), 0);

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
				return new Vector3(convert_units(aBTex.width/2.0*0.8),convert_units(-aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.RightHip)
				return new Vector3(convert_units(-aBTex.width/2.0*0.8),convert_units(-aBTex.height/2.0*0.8),0);
		}
		if(B == ZigJointId.Torso)
		{
			if(A == ZigJointId.Waist)
				return Vector3.zero;
			else if(A == ZigJointId.Neck)
				return new Vector3(0,convert_units(aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.LeftShoulder)
				return new Vector3(convert_units(aBTex.width/2.0*0.8),convert_units(aBTex.height/2.0*0.8),0);
			else if(A == ZigJointId.RightShoulder)
				return new Vector3(convert_units(-aBTex.width/2.0*0.8),convert_units(aBTex.height/2.0*0.8),0);
		}
		else if(A == ZigJointId.Torso)
		{
			return new Vector3(0,0,0); //TODO
		}
		else if(A != ZigJointId.None)
		{
			return new Vector3(0,-convert_units(aBTex.height/2.0 - aBTex.width*0.1),0);
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
		
		
		torso.transform.position = get_connection_point(ZigJointId.Torso,ZigJointId.Waist,aChar.waist);
		waist.transform.position = get_connection_point(ZigJointId.Waist,ZigJointId.Torso,aChar.torso);
		
		head.transform.parent = torso.transform;
		head.transform.position = head.transform.parent.position + get_connection_point(ZigJointId.Neck,ZigJointId.Torso,aChar.torso);
		
		leftUpperArm.transform.parent = torso.transform;
		leftUpperArm.transform.localPosition = leftUpperArm.transform.parent.global_scale().component_inverse().component_multiply(get_connection_point(ZigJointId.LeftShoulder,ZigJointId.Torso,aChar.torso));
		//leftUpperArm.transform.position = leftUpperArm.transform.parent.position + get_connection_point(ZigJointId.LeftShoulder,ZigJointId.Torso,aChar.torso);
		
		rightUpperArm.transform.parent = torso.transform;
		rightUpperArm.transform.position = rightUpperArm.transform.parent.position + get_connection_point(ZigJointId.RightShoulder,ZigJointId.Torso,aChar.torso);
		
		leftLowerArm.transform.parent = leftUpperArm.transform;
		//leftLowerArm.transform.localPosition = leftLowerArm.transform.parent.global_scale().component_inverse().component_multiply(get_connection_point(ZigJointId.LeftElbow,ZigJointId.LeftShoulder,aChar.leftUpperArm));
		leftLowerArm.transform.position = leftLowerArm.transform.parent.position + get_connection_point(ZigJointId.LeftElbow,ZigJointId.LeftShoulder,aChar.leftUpperArm);
		
		rightLowerArm.transform.parent = rightUpperArm.transform;
		rightLowerArm.transform.position = rightLowerArm.transform.parent.position + get_connection_point(ZigJointId.RightElbow,ZigJointId.RightShoulder,aChar.rightUpperArm);
		
		leftUpperLeg.transform.parent = waist.transform;
		leftUpperLeg.transform.position = leftUpperLeg.transform.parent.position + get_connection_point(ZigJointId.LeftHip,ZigJointId.Waist,aChar.torso);
		
		rightUpperLeg.transform.parent = waist.transform;
		rightUpperLeg.transform.position = rightUpperLeg.transform.parent.position + get_connection_point(ZigJointId.RightHip,ZigJointId.Waist,aChar.torso);
		
		leftLowerLeg.transform.parent = leftUpperLeg.transform;
		leftLowerLeg.transform.position = leftLowerLeg.transform.parent.position + get_connection_point(ZigJointId.LeftElbow,ZigJointId.LeftHip,aChar.leftUpperLeg);
		
		rightLowerLeg.transform.parent = rightUpperLeg.transform;
		rightLowerLeg.transform.position = rightLowerLeg.transform.parent.position + get_connection_point(ZigJointId.RightElbow,ZigJointId.RightHip,aChar.rightUpperLeg);
		
		
		List<GameObject> rotateMe;
		rotateMe.Add(leftUpperArm);
		rotateMe.Add(rightUpperArm);
		//rotateMe.Add(leftUpperArm);
		//rotateMe.Add(leftUpperArm);
        GameObject tempParent = new GameObject("genTempParent");
        kid.transform.parent = tempParent.transform;
        if (aId == ZigJointId.LeftShoulder || aId == ZigJointId.RightShoulder || aId == ZigJointId.LeftHip || aId == ZigJointId.RightHip || aId == ZigJointId.Neck || aId == ZigJointId.Waist)
            tempParent.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward) * tempParent.transform.rotation;
		kid.transform.parent = parent.transform;
        GameObject.Destroy(tempParent);
		
	}
}
