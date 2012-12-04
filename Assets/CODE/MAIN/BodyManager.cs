using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyManager : FakeMonoBehaviour {
	public BodyManager(ManagerManager aManager) : base(aManager) {}

    public void move_center(Vector3 pos)
    {
        if (mStartingTorso.sqrMagnitude == Mathf.Infinity)
            mStartingTorso = mParts[ZigJointId.Torso].transform.position;
        if (mStartingWaist.sqrMagnitude == Mathf.Infinity)
            mStartingWaist = mParts[ZigJointId.Waist].transform.position;
        mParts[ZigJointId.Torso].transform.position = mStartingTorso + pos;
        mParts[ZigJointId.Waist].transform.position = mStartingWaist + pos;
    }
    public void set_character(CharacterTextureBehaviour aChar)
    {
        create_body(aChar);
    }

    public void set_transparent(ProGrading.Pose aPose)
    {
        //TODO
        mMode = 1;
        mTargetPose = aPose;
        foreach (GameObject e in mParts.Values)
        {
            foreach(Renderer f in e.GetComponentsInChildren<Renderer>())
            {
                if (f.gameObject.name == "Plane")
                {
                    Color c = f.material.GetColor("_TintColor");
                    c.a = 0.2f;
                    f.material.SetColor("_TintColor", c);
                }
            }
        }
    }

    public void destroy_character()
    {
        mTargetPose = null;
        //mMode = -1;
        foreach (GameObject e in mParts.Values)
            GameObject.Destroy(e);
        mParts.Clear();
    }

    public void character_changed_listener(CharacterTextureBehaviour aCharacter)
    {
        destroy_character();
        set_character(aCharacter);
        //reposition the characeter
        move_center(new Vector3(BodyManager.convert_units(aCharacter.background1.width) / 4.0f, 0, 0));
    }

    int mMode = 0; // 0 - from kinect, 1 - from pose, -1 none
    public ProGrading.Pose mTargetPose = null;
    public Vector3 mStartingTorso = new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
    public Vector3 mStartingWaist = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    Dictionary<ZigJointId, GameObject> mParts = new Dictionary<ZigJointId, GameObject>();
	
	public Vector3 get_offset_of_plane(Transform aGo)
	{
		Transform plane = aGo.FindChild("Plane");
		if(plane != null)
			return plane.position - aGo.transform.position;
		throw new UnityException("no plane child exsits");
	}

	public static float convert_units(double pixelWidth)
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
        throw new UnityException("color " + c.ToString() + " not found in texture " + aTex.name );
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
		//GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//sphere.transform.localScale = Vector3.one * 0.2f;
		//sphere.transform.parent = parent.transform;
		GameObject kid = GameObject.CreatePrimitive(PrimitiveType.Plane);
		kid.renderer.material = new Material(mManager.mReferences.mDefaultCharacterShader);
		kid.renderer.material.mainTexture = aTex;
        kid.transform.rotation =  Quaternion.AngleAxis(90, Vector3.right) * kid.transform.rotation;
        
        kid.transform.localScale = new Vector3(convert_units(aTex.width) / 10.0f, 1, convert_units(aTex.height) / 10.0f);
        kid.transform.position = -get_attachment_point(0, aAttachTex);
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

    public Vector3 get_Z_offset(ZigJointId id)
    {
        switch (id)
        {
            case ZigJointId.RightElbow:
                return new Vector3(0, 0, -0.0f);
            case ZigJointId.LeftElbow:
                return new Vector3(0, 0, -0.1f);
            case ZigJointId.RightShoulder:
                return new Vector3(0, 0, -0.2f);
            case ZigJointId.LeftShoulder:
                return new Vector3(0, 0, -0.3f);
            case ZigJointId.Torso:
                return new Vector3(0, 0, -0.4f);
            case ZigJointId.Waist:
                return new Vector3(0, 0, -0.5f);
            case ZigJointId.LeftHip:
                return new Vector3(0, 0, -0.6f);
            case ZigJointId.RightHip:
                return new Vector3(0, 0, -0.7f);
            case ZigJointId.RightKnee:
                return new Vector3(0, 0, -0.8f);
            case ZigJointId.LeftKnee:
                return new Vector3(0, 0, -0.9f);
        }
        return Vector3.zero;
    }

	public override void Start () 
    {
        
        mManager.mEventManager.character_changed_event += character_changed_listener;
	}

    public override void Update()
    {
        if (mMode == 0)
        {
            //grade
            if(mManager.mZigManager.has_user())
                mManager.mInterfaceManager.mGrade = ProGrading.grade_pose(ProGrading.snap_pose(mManager), mManager.mTransparentBodyManager.mTargetPose);

            foreach (KeyValuePair<GradingManager.WeightedZigJointPair, ProjectionManager.Smoothing> e in mManager.mProjectionManager.mImportant)
            {
                mParts[e.Key.A].transform.rotation = Quaternion.AngleAxis(e.Value.current, Vector3.forward);
            }
            mParts[ZigJointId.Waist].transform.rotation = Quaternion.AngleAxis(mManager.mProjectionManager.mWaist.current,Vector3.forward);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(mManager.mZigManager.has_user())
                    ProGrading.write_pose_to_file(ProGrading.snap_pose(mManager), "char_kinect.txt");
            }
        }
        else if (mMode == 1)
        {
            if (mTargetPose != null && mManager.mRecordMode == false)
            {
                foreach (ProGrading.PoseElement e in mTargetPose.mElements)
                {
                    mParts[e.joint].transform.rotation = Quaternion.AngleAxis(e.angle, Vector3.forward);
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ProGrading.Pose p = new ProGrading.Pose();
                foreach (KeyValuePair<ZigJointId, GameObject> e in mParts)
                {
                    ProGrading.PoseElement pe = new ProGrading.PoseElement();
                    pe.joint = e.Key;
                    pe.angle = e.Value.transform.rotation.eulerAngles.z;
                    p.mElements.Add(pe);
                }
                ProGrading.write_pose_to_file(p, "char_manual.txt");
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

        //order things


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
        torso.transform.position += get_Z_offset(ZigJointId.Torso);
        waist.transform.position += get_Z_offset(ZigJointId.Waist);

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
                + get_connection_point_image(e.Key, e.Value, jointTexture[e.Value])
                + get_Z_offset(e.Key)
                - get_Z_offset(e.Value);
                
		}

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
		rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm,-90));
		rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm,-90));
        
		rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg,-90));
		rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg,-90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso,90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(waist, -90));
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
