using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}

    public void set_character(CharacterTextureBehaviour aChar)
    {
        //TODO
    }

    public void set_transparent(GradingManager.Pose aPose)
    {
        //TODO
        mMode = 1;
        mTargetPose = aPose;
        foreach (GameObject e in mParts.Values)
        {
            foreach(Renderer f in e.GetComponentsInChildren<Renderer>())
            {
                Color c = f.material.color;
                c.a = 0.5f;
                f.material.color = c;
            }
        }
    }

    public void destroy_character()
    {
        mMode = -1;
        foreach (GameObject e in mParts.Values)
            GameObject.Destroy(e);
    }

    int mMode = 1; // 0 - from kinect, 1 - from pose, 2 - record pose, -1 none
    public GradingManager.Pose mTargetPose = null;
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

    public bool is_same_color(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
    public bool is_close_color(Color32 c1, Color32 c2)
    {
        return Mathf.Max(Mathf.Abs(c1.r - c2.r), Mathf.Max(Mathf.Abs(c1.g - c2.g), Mathf.Abs(c1.b - c2.b))) < 40;
    }
    public bool is_stupid_color(Color32 c1, Color32 c2)
    {
        return ((c1.r != 255 && c2.r != 255) || (c1.r == 255 && c2.r == 255)) &&
            ((c1.g != 255 && c2.g != 255) || (c1.g == 255 && c2.g == 255)) &&
            ((c1.b != 255 && c2.b != 255) || (c1.b == 255 && c2.b == 255));
    }
    public Vector3 index_to_position(int i, Texture2D aTex)
    {
        
        int x = i % aTex.width - aTex.width/2;
        int y = i / aTex.width - aTex.height/2;

        return new Vector3(-convert_units(x), convert_units(y));
    }

    public Vector3 find_first_color(Color32 c, Texture2D aTex)
    {
        
        Color32[] colors = aTex.GetPixels32();
        for (int i = 0; i < colors.Length; i++)
        { 
            if (is_same_color(colors[i], c))
            {
            
                return index_to_position(i, aTex);
            }
        }
        //return Vector3.zero;
        throw new UnityException("color " + c.ToString() + " not found");
    }

    public Vector3 get_attachment_point(int aId, Texture2D aTex)
    {
        Color32 c;
        switch (aId)
        {
            case 0:
                c = new Color32(255, 0, 0,255);
                break;
            case 1:
                c = new Color32(0, 255, 0,255);
                break;
            case 2:
                c = new Color32(0, 0, 255,255);
                break;
            case 3:
                c = new Color32(255, 255, 0,255);
                break;
            default:
                return Vector3.zero;
        }
        return find_first_color(c, aTex);
    }

	public GameObject create_object(ZigJointId aId, Texture2D aTex, Texture2D aAttachTex)
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
        kid.transform.position = -get_attachment_point(0, aAttachTex);
        /*
        if (aId == ZigJointId.Neck || aId == ZigJointId.Torso)
            kid.transform.position = new Vector3(0, convert_units(aTex.height / 2.0f * 0.8f), 0);
        if (aId == ZigJointId.LeftShoulder || aId == ZigJointId.RightShoulder || aId == ZigJointId.LeftHip || aId == ZigJointId.RightHip || 
            aId == ZigJointId.LeftElbow || aId == ZigJointId.RightElbow || aId == ZigJointId.LeftKnee || aId == ZigJointId.RightKnee || aId == ZigJointId.Waist)
            kid.transform.position = new Vector3(0, convert_units(-aTex.height / 2.0f * 0.8f), 0);
        */
        kid.transform.parent = parent.transform;

		mParts[aId] = parent;
		return parent;
	}

    public Vector3 get_connection_point_image(ZigJointId A, ZigJointId B, Texture2D aBTex)
    {
        if(A == B)
        {
            return get_attachment_point(0,aBTex);
        }
        if(B == ZigJointId.Waist)
        {
            if(A == ZigJointId.Torso)
                return get_attachment_point(0,aBTex);
            else if(A == ZigJointId.LeftHip)
                return get_attachment_point(1,aBTex);
            else if(A == ZigJointId.RightHip)
                return get_attachment_point(2,aBTex);
        }
        else if(B == ZigJointId.Torso)
        {
            if(A == ZigJointId.Neck)
                return get_attachment_point(3,aBTex);
            else if(A == ZigJointId.LeftShoulder)
                return get_attachment_point(1,aBTex);
            else if(A == ZigJointId.RightShoulder)
                return get_attachment_point(2,aBTex);
        }
        else
        {
            return get_attachment_point(1, aBTex);
        }
        throw new UnityException("uh oh, can't find attachment point zigjointid map");
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
        if (mMode == 0)
        {
            foreach (KeyValuePair<GradingManager.WeightedZigJointPair, ProjectionManager.Smoothing> e in mManager.mProjectionManager.mImportant)
            {
                if (e.Key.A == ZigJointId.None)
                {
                    //TODO
                }
                else
                {
                    mParts[e.Key.A].transform.rotation = Quaternion.AngleAxis(e.Value.current, Vector3.forward);
                }
            }
        }
        else if (mMode == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GradingManager.Pose p = mManager.mGradingManager.read_pose(mManager.mReferences.mDemoChar.GetComponent<CharacterTextureBehaviour>().properPose);
                mTargetPose = p;
            }
            if (mTargetPose != null)
            {
                foreach (KeyValuePair<GradingManager.WeightedZigJointPair, ProjectionManager.Smoothing> e in mManager.mProjectionManager.mImportant)
                {
                    if (e.Key.A != ZigJointId.None)
                    {
                        mParts[e.Key.A].transform.rotation = Quaternion.AngleAxis(mManager.mProjectionManager.get_relative(mTargetPose.mPose[e.Key.A], mTargetPose.mPose[e.Key.B]), Vector3.forward);
                    }
                }
            }
        }
        else if (mMode == 2)
        { 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GradingManager.Pose p = new GradingManager.Pose();
                foreach(KeyValuePair<ZigJointId,GameObject> e in mParts)
                {
                    ZigInputJoint joint = new ZigInputJoint(e.Key);
                    joint.GoodPosition = true;
                    joint.Position = e.Value.transform.position;
                    p.mPose[e.Key] = joint;
                }
                List<KeyValuePair<ZigJointId, ZigJointId>> toConstruct = new List<KeyValuePair<ZigJointId, ZigJointId>>();
                toConstruct.Add(new KeyValuePair < ZigJointId, ZigJointId > (ZigJointId.Neck, ZigJointId.Head));
                toConstruct.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftElbow, ZigJointId.LeftHand));
                toConstruct.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightElbow, ZigJointId.RightHand));
                toConstruct.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.LeftKnee, ZigJointId.LeftAnkle));
                toConstruct.Add(new KeyValuePair<ZigJointId, ZigJointId>(ZigJointId.RightKnee, ZigJointId.RightAnkle));
                foreach (KeyValuePair<ZigJointId, ZigJointId> e in toConstruct)
                {
                    ZigInputJoint joint = new ZigInputJoint(e.Key);
                    joint.GoodPosition = true;
                    Vector3 dir = new Vector3(Mathf.Cos(mParts[e.Key].transform.rotation.eulerAngles.z),Mathf.Sin(mParts[e.Key].transform.rotation.eulerAngles.z),0);
                    joint.Position = mParts[e.Key].transform.position + dir;
                    p.mPose[e.Value] = joint;
                }
                mManager.mGradingManager.write_pose_to_file(p, "princess.txt");
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                GradingManager.Pose p = mManager.mGradingManager.read_pose(mManager.mReferences.mDemoChar.GetComponent<CharacterTextureBehaviour>().properPose);
                mTargetPose = p;
                //Debug.Log("pose is " + mManager.mGradingManager.grade_pose(p));
                mManager.mGradingManager.print_pose(p);
            }
        }
	}
	
	public void create_body(CharacterTextureBehaviour aChar)
	{
		GameObject torso = create_object(ZigJointId.Torso,aChar.torso,aChar.atTorso);
        GameObject waist = create_object(ZigJointId.Waist, aChar.waist, aChar.atWaist);
        GameObject head = create_object(ZigJointId.Neck, aChar.head, aChar.atHead);
        GameObject leftUpperArm = create_object(ZigJointId.LeftShoulder, aChar.leftUpperArm, aChar.atLeftUpperArm);
        GameObject rightUpperArm = create_object(ZigJointId.RightShoulder, aChar.rightUpperArm, aChar.atRightUpperArm);
        GameObject leftLowerArm = create_object(ZigJointId.LeftElbow, aChar.leftLowerArm, aChar.atLeftLowerArm);
        GameObject rightLowerArm = create_object(ZigJointId.RightElbow, aChar.rightLowerArm, aChar.atRightLowerArm);
        GameObject leftUpperLeg = create_object(ZigJointId.LeftHip, aChar.leftUpperLeg, aChar.atLeftUpperLeg);
        GameObject rightUpperLeg = create_object(ZigJointId.RightHip, aChar.rightUpperLeg, aChar.atRightUpperLeg);
        GameObject leftLowerLeg = create_object(ZigJointId.LeftKnee, aChar.leftLowerLeg, aChar.atLeftLowerLeg);
        GameObject rightLowerLeg = create_object(ZigJointId.RightKnee, aChar.rightLowerLeg, aChar.atRightLowerLeg);

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
		
        /*
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
		*/

        jointTexture[ZigJointId.Torso] = aChar.atTorso;
        jointTexture[ZigJointId.Waist] = aChar.atWaist;
        jointTexture[ZigJointId.Neck] = aChar.atHead;
        jointTexture[ZigJointId.LeftShoulder] = aChar.atLeftUpperArm;
        jointTexture[ZigJointId.RightShoulder] = aChar.atRightUpperArm;
        jointTexture[ZigJointId.LeftElbow] = aChar.atLeftLowerArm;
        jointTexture[ZigJointId.RightElbow] = aChar.atRightLowerArm;
        jointTexture[ZigJointId.LeftHip] = aChar.atLeftUpperLeg;
        jointTexture[ZigJointId.RightHip] = aChar.atRightUpperLeg;
        jointTexture[ZigJointId.LeftKnee] = aChar.atLeftLowerLeg;
        jointTexture[ZigJointId.RightKnee] = aChar.atRightLowerLeg;


        //these two are special
        torso.transform.position = waist.transform.position;

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
		
		foreach(KeyValuePair<ZigJointId,ZigJointId> e in relations)
		{
			jointObject[e.Key].transform.parent = jointObject[e.Value].transform;
            jointObject[e.Key].transform.position =
                jointObject[e.Value].transform.position
                + get_offset_of_plane(jointObject[e.Value].transform)
                + get_connection_point_image(e.Key, e.Value, jointTexture[e.Value]);
				//+ get_connection_point(e.Key,e.Value,jointTexture[e.Value]);
                
		}

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
		rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm,-90));
		rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm,-90));
        
		rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg,-90));
		rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(head,90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm,-90));
		
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
