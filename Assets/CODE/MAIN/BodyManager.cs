using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}

    Dictionary<ZigJointId, GameObject> mParts = new Dictionary<ZigJointId, GameObject>();
	
	
	public Vector3 get_offset_of_plane(Transform aGo)
	{
		Transform plane = aGo.FindChild("Plane");
		if(plane != null)
			return plane.position - aGo.transform.position;
		throw new UnityException("no plane child exsits");
	}
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
		kid.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
		kid.renderer.material.mainTexture = aTex;
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
				return Vector3.zero;
				//return new Vector3(0,convert_units(aBTex.height/2.0*0.8),0);
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
			//Debug.Log (A.ToString() + " " + B.ToString());
			return new Vector3(0,convert_units(-aBTex.height/2.0f*0.8f),0);
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
		
		Dictionary<ZigJointId, GameObject> jointObject = new Dictionary<ZigJointId, GameObject>();
		Dictionary<ZigJointId, Texture2D> jointTexture = new Dictionary<ZigJointId, Texture2D>();
		jointObject[ZigJointId.Torso] = torso;
		jointObject[ZigJointId.Waist] = waist;
		jointObject[ZigJointId.Neck] = head;
		jointObject[ZigJointId.LeftShoulder] = leftUpperArm;
		jointObject[ZigJointId.RightShoulder] = rightUpperArm;
		jointObject[ZigJointId.LeftElbow] = leftLowerArm;
		jointObject[ZigJointId.RightElbow] = rightLowerArm;
		jointObject[ZigJointId.LeftHip] = leftUpperLeg;
		jointObject[ZigJointId.RightHip] = rightUpperLeg;
		jointObject[ZigJointId.LeftKnee] = leftLowerLeg;
		jointObject[ZigJointId.RightKnee] = rightLowerLeg;
		
		jointTexture[ZigJointId.Torso] = aChar.torso;
		jointTexture[ZigJointId.Waist] = aChar.waist;
		jointTexture[ZigJointId.Neck] = aChar.head;
		jointTexture[ZigJointId.LeftShoulder] = aChar.leftUpperArm;
		jointTexture[ZigJointId.RightShoulder] = aChar.rightUpperArm;
		jointTexture[ZigJointId.LeftElbow] = aChar.leftLowerArm;
		jointTexture[ZigJointId.RightElbow] = aChar.rightLowerArm;
		jointTexture[ZigJointId.LeftHip] = aChar.leftUpperLeg;
		jointTexture[ZigJointId.RightHip] = aChar.rightUpperLeg;
		jointTexture[ZigJointId.LeftKnee] = aChar.leftLowerLeg;
		jointTexture[ZigJointId.RightKnee] = aChar.rightLowerLeg;
		
		List<KeyValuePair<ZigJointId,ZigJointId>> relations = new List<KeyValuePair<ZigJointId, ZigJointId>>();
		
		
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.LeftShoulder,ZigJointId.Torso));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.RightShoulder,ZigJointId.Torso));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.LeftElbow,ZigJointId.LeftShoulder));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.RightElbow,ZigJointId.RightShoulder));
		
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.LeftHip,ZigJointId.Waist));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.RightHip,ZigJointId.Waist));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.LeftKnee,ZigJointId.LeftHip));
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.RightKnee,ZigJointId.RightHip));
		
		relations.Add(new KeyValuePair<ZigJointId,ZigJointId>(ZigJointId.Neck,ZigJointId.Torso));
		
		//these two are special
		torso.transform.position = get_connection_point(ZigJointId.Torso,ZigJointId.Waist,aChar.waist);
		waist.transform.position = get_connection_point(ZigJointId.Waist,ZigJointId.Torso,aChar.torso);
		
		foreach(KeyValuePair<ZigJointId,ZigJointId> e in relations)
		{
			jointObject[e.Key].transform.parent = jointObject[e.Value].transform;
			jointObject[e.Key].transform.position = 
				jointObject[e.Value].transform.position
				+ get_offset_of_plane(jointObject[e.Value].transform)
				+ get_connection_point(e.Key,e.Value,jointTexture[e.Value]);
		}

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
		//rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm,-90));
		//rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm,-90));
        
		rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg,-90));
		rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(head, -90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg,180));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg,180));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm,180));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm,180));
		//rotateMe.Add(waist);
        foreach (KeyValuePair<GameObject, float> e in rotateMe)
		{
			GameObject tempParent = new GameObject("genTempParent");
			tempParent.transform.position = e.Key.transform.position;
			List<Transform> children = new List<Transform>();
			for(int i = 0; i < e.Key.transform.GetChildCount(); i++)
				if(e.Key.transform.GetChild(i).parent == e.Key.transform)
					children.Add(e.Key.transform.GetChild(i));
			foreach(Transform f in children)
				f.parent = tempParent.transform;
			tempParent.transform.rotation = Quaternion.AngleAxis(e.Value, Vector3.forward) * tempParent.transform.rotation;
			foreach(Transform f in children)
				f.parent = e.Key.transform;
			GameObject.Destroy(tempParent);
		}
	}
}
