using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class FlatBodyObject : FlatElementBase
{

    public static ZgJointId[] mRenderedJoints = { ZgJointId.Neck, ZgJointId.LeftElbow, ZgJointId.LeftKnee, ZgJointId.LeftShoulder, ZgJointId.LeftHip, ZgJointId.RightElbow, ZgJointId.RightKnee, ZgJointId.RightShoulder, ZgJointId.RightHip, ZgJointId.Torso, ZgJointId.Waist };
    public int get_joint_alpha_index(ZgJointId aId)
    {
        int r = -1;
        for (int i = 0; i < mRenderedJoints.Length; i++)
            if (mRenderedJoints[i] == aId)
                r = i;
        return r;
    }
	
    public Pose mTargetPose = null;
    public Dictionary<ZgJointId, GameObject> mParts = new Dictionary<ZgJointId, GameObject>();
    Vector3 mOffset = Vector3.zero;
    public bool UseDepth { get; set; }


    public FlatBodyObject(CharacterTextureBehaviour aChar, int aDepth = -1)
    {
        create_body(aChar);
        PrimaryGameObject = new GameObject("genBodyParent");
        mParts[ZgJointId.Waist].transform.parent = PrimaryGameObject.transform;
        mParts[ZgJointId.Torso].transform.parent = PrimaryGameObject.transform;
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        SoftInterpolation = 0.08f;

        UseDepth = (aDepth != -1);
        if (UseDepth)
            Depth = aDepth;
    }

    public FlatBodyObject(CharacterLoader aChar, int aDepth = -1)
    {
		//hacky head reorder code
		bool reorder = false;
		if(ManagerManager.Manager.mCharacterBundleManager.get_character_helper() != null)
			if(ManagerManager.Manager.mCharacterBundleManager.get_character_stat(aChar.Character).CharacterInfo.Order != 0)
					reorder = true;

        //TODO fake thread this eventually please..
		foreach (FlatBodyObject e in load_sequential(aChar.Images, aChar.Sizes, reorder))
			;

        mOffset = (new Vector3(aChar.Sizes.mOffset.x, aChar.Sizes.mOffset.y, 0));

        PrimaryGameObject = new GameObject("genBodyParent");
        mParts[ZgJointId.Waist].transform.parent = PrimaryGameObject.transform;
        mParts[ZgJointId.Torso].transform.parent = PrimaryGameObject.transform;
        SoftColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        SoftInterpolation = 0.08f;

        UseDepth = (aDepth != -1);
        if (UseDepth)
            Depth = aDepth;
    }
	
	public Vector3 get_body_part_position(ZgJointId aJoint)
	{
		return mParts[aJoint].transform.position;
	}
	
	public Bounds get_body_bounds()
	{
		Bounds r = mParts[ZgJointId.Torso].transform.position.to_bounds();
		foreach(var e in mParts.Values)
		{
			r = r.union(e.transform.position);
		}
		return r;
	}
	
	public Pose get_pose()
	{
		Pose p = new Pose();
        foreach (KeyValuePair<ZgJointId, GameObject> e in mParts)
        {
			if( e.Key != ZgJointId.LeftHand &&
				e.Key != ZgJointId.RightHand &&
				e.Key != ZgJointId.LeftAnkle &&
				e.Key != ZgJointId.RightAnkle &&
				e.Key != ZgJointId.Head )
			{
	            PoseElement pe = new PoseElement();
	            pe.joint = e.Key;
	            pe.angle = e.Value.transform.rotation.eulerAngles.z;
	            p.mElements.Add(pe);
			}
        }	
        return p;
	}


	public void reorder_head_first()
	{
	}

    public void match_body_location_to_projection(ZgManager aManager)
    {
        Vector3 position = Vector3.zero;
        if (!aManager.using_nite())
            position = aManager.Joints[ZgJointId.Waist].Position;
        else position = aManager.Joints[ZgJointId.Torso].Position;
        position.z = 0;
        position.y = 0;
        position.x *= -1;
        if (Mathf.Abs(position.x) < 10) position.x = 0; //fake snapping, TODO this should probbaly be in grading manager if anywhere...
        SoftPosition = position / 1.5f + mOffset;
    }
	
	public void match_body_to_body(FlatBodyObject aTarget)
	{
		foreach (var e in aTarget.mParts)
		{
            mParts[e.Key].transform.rotation = e.Value.transform.rotation;//Quaternion.Slerp(mParts[e.Key].transform.rotation,e.Value.transform.rotation,0.2f);
        }
	}
    public void match_body_to_projection(ProjectionManager aManager)
    {
        foreach (KeyValuePair<ZgJointId, ProjectionManager.Stupid> e in aManager.mImportant)
        {
            mParts[e.Key].transform.rotation = Quaternion.AngleAxis(e.Value.smoothing.current, Vector3.forward);
        }
        mParts[ZgJointId.Waist].transform.rotation = Quaternion.AngleAxis(aManager.mWaist.current, Vector3.forward);
    }

    public void set_target_pose(Pose aPose, bool hard = false)
    {
        mTargetPose = aPose;
		if(hard)
			update_parameters_impl(0,1);
    }

	public void set_layer(int layer)
	{
		foreach (GameObject e in mParts.Values)
		{
			foreach (Renderer f in e.GetComponentsInChildren<Renderer>())
			{
				f.gameObject.layer = layer;
			}
		}
	}

	
	
    public override void update_parameters(float aDeltaTime)
    {
       update_parameters_impl(aDeltaTime);
    }
	public void update_parameters_impl(float aDeltaTime, float interp = 0.2f)
	{
		if (mTargetPose != null)
        {
            foreach (PoseElement e in mTargetPose.mElements)
            {
                mParts[e.joint].transform.rotation = Quaternion.Slerp(mParts[e.joint].transform.rotation, Quaternion.AngleAxis(e.angle, Vector3.forward), interp);
            }
        }
        base.update_parameters(aDeltaTime);
	}

    public override void destroy()
    {
        mTargetPose = null;
        //mMode = -1;
        foreach (GameObject e in mParts.Values)
            GameObject.Destroy(e);
        mParts.Clear();
		
		GameObject.Destroy(PrimaryGameObject);
    }


    public IEnumerable<FlatBodyObject> load_sequential(CharacterData.CharacterDataImages aImages, CharacterData.CharacterDataSizes aSizes, bool useHeadOrder = false)
    {
        
        GameObject neck = create_object(ZgJointId.Neck, aImages.head, aSizes.mLimbSizes[0], aSizes.mMountingPositions[0]);
        yield return null;
        GameObject leftLowerArm = create_object(ZgJointId.LeftElbow, aImages.leftLowerArm, aSizes.mLimbSizes[1], aSizes.mMountingPositions[1]);
        yield return null;
        GameObject leftLowerLeg = create_object(ZgJointId.LeftKnee, aImages.leftLowerLeg, aSizes.mLimbSizes[2], aSizes.mMountingPositions[2]);
        yield return null;
        GameObject leftUpperArm = create_object(ZgJointId.LeftShoulder, aImages.leftUpperArm, aSizes.mLimbSizes[3], aSizes.mMountingPositions[3]);
        yield return null;
        GameObject leftUpperLeg = create_object(ZgJointId.LeftHip, aImages.leftUpperLeg, aSizes.mLimbSizes[4], aSizes.mMountingPositions[4]);
        yield return null;
        GameObject rightLowerArm = create_object(ZgJointId.RightElbow, aImages.rightLowerArm, aSizes.mLimbSizes[5], aSizes.mMountingPositions[5]);
        yield return null;
        GameObject rightLowerLeg = create_object(ZgJointId.RightKnee, aImages.rightLowerLeg, aSizes.mLimbSizes[6], aSizes.mMountingPositions[6]);
        yield return null;
        GameObject rightUpperArm = create_object(ZgJointId.RightShoulder, aImages.rightUpperArm, aSizes.mLimbSizes[7], aSizes.mMountingPositions[7]);
        yield return null;
        GameObject rightUpperLeg = create_object(ZgJointId.RightHip, aImages.rightUpperLeg, aSizes.mLimbSizes[8], aSizes.mMountingPositions[8]);
        yield return null;
        GameObject torso = create_object(ZgJointId.Torso, aImages.torso, aSizes.mLimbSizes[9], aSizes.mMountingPositions[9]);
        yield return null;
        GameObject waist = create_object(ZgJointId.Waist, aImages.waist, aSizes.mLimbSizes[10], aSizes.mMountingPositions[10]);
        yield return null;
		
		
		GameObject leftHand = create_extremety(ZgJointId.LeftHand);
		GameObject rightHand = create_extremety(ZgJointId.RightHand);
		GameObject leftAnkle = create_extremety(ZgJointId.LeftAnkle);
		GameObject rightAnkle = create_extremety(ZgJointId.RightAnkle);
		GameObject head = create_extremety(ZgJointId.Head);
		
		

        //order things
        Dictionary<ZgJointId, GameObject> jointObject = new Dictionary<ZgJointId, GameObject>();
        //Dictionary<ZgJointId, Texture2D> jointTexture = new Dictionary<ZgJointId, Texture2D>();
        jointObject[ZgJointId.Torso] = torso;
        jointObject[ZgJointId.Waist] = waist;
        jointObject[ZgJointId.Neck] = neck;
        jointObject[ZgJointId.LeftShoulder] = leftUpperArm;
        jointObject[ZgJointId.RightShoulder] = rightUpperArm;
        jointObject[ZgJointId.LeftElbow] = leftLowerArm;
        jointObject[ZgJointId.RightElbow] = rightLowerArm;
        jointObject[ZgJointId.LeftHip] = leftUpperLeg;
        jointObject[ZgJointId.RightHip] = rightUpperLeg;
        jointObject[ZgJointId.LeftKnee] = leftLowerLeg;
        jointObject[ZgJointId.RightKnee] = rightLowerLeg;
		
		jointObject[ZgJointId.LeftHand] = leftHand;
		jointObject[ZgJointId.RightHand] = rightHand;
		jointObject[ZgJointId.LeftAnkle] = leftAnkle;
		jointObject[ZgJointId.RightAnkle] = rightAnkle;
		jointObject[ZgJointId.Head] = head;

        //these two are special
        torso.transform.position = waist.transform.position;
		//we don't check if we use headfirst z offset here because it's the same in both cases...
        torso.transform.position += get_Z_offset(ZgJointId.Torso); 
        waist.transform.position += get_Z_offset(ZgJointId.Waist);
		//torso.GetComponentInChildren<Renderer>().material.renderQueue = (int)(get_Z_offset(ZigJointId.Torso).z*(10));
		//waist.GetComponentInChildren<Renderer>().material.renderQueue = (int)(get_Z_offset(ZigJointId.Waist).z*(10));

        List<KeyValuePair<ZgJointId, ZgJointId>> relations = new List<KeyValuePair<ZgJointId, ZgJointId>>();
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftShoulder, ZgJointId.Torso));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightShoulder, ZgJointId.Torso));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftElbow, ZgJointId.LeftShoulder));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightElbow, ZgJointId.RightShoulder));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftHip, ZgJointId.Waist));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightHip, ZgJointId.Waist));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftKnee, ZgJointId.LeftHip));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightKnee, ZgJointId.RightHip));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.Neck, ZgJointId.Torso));
		
		
		
		relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftHand, ZgJointId.LeftElbow));
		relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightHand, ZgJointId.RightElbow));
		relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftAnkle, ZgJointId.LeftKnee));
		relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightAnkle, ZgJointId.RightKnee));
		relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.Head, ZgJointId.Neck));
		 

        foreach (KeyValuePair<ZgJointId, ZgJointId> e in relations)
        {
			
            jointObject[e.Key].transform.parent = jointObject[e.Value].transform;
			
			
			try{
            jointObject[e.Key].transform.position =
                jointObject[e.Value].transform.position
                + get_offset_of_plane(jointObject[e.Value].transform)
                + get_connection_point_list(e.Key, e.Value, aSizes.mMountingPositions[get_joint_alpha_index(e.Value)])
				+ (useHeadOrder ? 
						get_headfirst_Z_offset(e.Key) - get_headfirst_Z_offset(e.Value) : 
						get_Z_offset(e.Key) - get_Z_offset(e.Value));
			//jointObject[e.Key].GetComponentInChildren<Renderer>().material.renderQueue = (int)(get_Z_offset(e.Key).z*(10));
			}
			catch
			{
				//this should only happen for missing hand dots right now
				//put the hand on the elbow
				jointObject[e.Key].transform.position = jointObject[e.Value].transform.position;
			}

        }

        yield return null;

        List<KeyValuePair<GameObject, float>> rotateMe = new List<KeyValuePair<GameObject, float>>();
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm, -90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso, 90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(waist, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(neck, 90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm, -90));

        foreach (KeyValuePair<GameObject, float> e in rotateMe)
        {
            GameObject tempParent = new GameObject("genTempParent");
            tempParent.transform.position = e.Key.transform.position;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < e.Key.transform.childCount; i++)
                if (e.Key.transform.GetChild(i).parent == e.Key.transform)
                    children.Add(e.Key.transform.GetChild(i));
            foreach (Transform f in children)
                f.parent = tempParent.transform;
            tempParent.transform.rotation = Quaternion.AngleAxis(e.Value, Vector3.forward) * tempParent.transform.rotation;
            foreach (Transform f in children)
                f.parent = e.Key.transform;
            GameObject.Destroy(tempParent);
        }

        yield return this;
    }



    //useful
	//NOTE this does not support extremeties
    void create_body(CharacterTextureBehaviour aChar)
    {
        GameObject torso = create_object(ZgJointId.Torso, aChar.torso, aChar.atTorso);
        GameObject waist = create_object(ZgJointId.Waist, aChar.waist, aChar.atWaist);
        GameObject head = create_object(ZgJointId.Neck, aChar.head, aChar.atHead);
        GameObject leftUpperArm = create_object(ZgJointId.LeftShoulder, aChar.leftUpperArm, aChar.atLeftUpperArm);
        GameObject rightUpperArm = create_object(ZgJointId.RightShoulder, aChar.rightUpperArm, aChar.atRightUpperArm);
        GameObject leftLowerArm = create_object(ZgJointId.LeftElbow, aChar.leftLowerArm, aChar.atLeftLowerArm);
        GameObject rightLowerArm = create_object(ZgJointId.RightElbow, aChar.rightLowerArm, aChar.atRightLowerArm);
        GameObject leftUpperLeg = create_object(ZgJointId.LeftHip, aChar.leftUpperLeg, aChar.atLeftUpperLeg);
        GameObject rightUpperLeg = create_object(ZgJointId.RightHip, aChar.rightUpperLeg, aChar.atRightUpperLeg);
        GameObject leftLowerLeg = create_object(ZgJointId.LeftKnee, aChar.leftLowerLeg, aChar.atLeftLowerLeg);
        GameObject rightLowerLeg = create_object(ZgJointId.RightKnee, aChar.rightLowerLeg, aChar.atRightLowerLeg);

        //order things
        Dictionary<ZgJointId, GameObject> jointObject = new Dictionary<ZgJointId, GameObject>();
        Dictionary<ZgJointId, Texture2D> jointTexture = new Dictionary<ZgJointId, Texture2D>();
        jointObject[ZgJointId.Torso] = torso;
        jointObject[ZgJointId.Waist] = waist;
        jointObject[ZgJointId.Neck] = head;
        jointObject[ZgJointId.LeftShoulder] = leftUpperArm;
        jointObject[ZgJointId.RightShoulder] = rightUpperArm;
        jointObject[ZgJointId.LeftElbow] = leftLowerArm;
        jointObject[ZgJointId.RightElbow] = rightLowerArm;
        jointObject[ZgJointId.LeftHip] = leftUpperLeg;
        jointObject[ZgJointId.RightHip] = rightUpperLeg;
        jointObject[ZgJointId.LeftKnee] = leftLowerLeg;
        jointObject[ZgJointId.RightKnee] = rightLowerLeg;


        jointTexture[ZgJointId.Torso] = aChar.atTorso;
        jointTexture[ZgJointId.Waist] = aChar.atWaist;
        jointTexture[ZgJointId.Neck] = aChar.atHead;
        jointTexture[ZgJointId.LeftShoulder] = aChar.atLeftUpperArm;
        jointTexture[ZgJointId.RightShoulder] = aChar.atRightUpperArm;
        jointTexture[ZgJointId.LeftElbow] = aChar.atLeftLowerArm;
        jointTexture[ZgJointId.RightElbow] = aChar.atRightLowerArm;
        jointTexture[ZgJointId.LeftHip] = aChar.atLeftUpperLeg;
        jointTexture[ZgJointId.RightHip] = aChar.atRightUpperLeg;
        jointTexture[ZgJointId.LeftKnee] = aChar.atLeftLowerLeg;
        jointTexture[ZgJointId.RightKnee] = aChar.atRightLowerLeg;


        //these two are special
        torso.transform.position = waist.transform.position;
        torso.transform.position += get_Z_offset(ZgJointId.Torso);
        waist.transform.position += get_Z_offset(ZgJointId.Waist);

        List<KeyValuePair<ZgJointId, ZgJointId>> relations = new List<KeyValuePair<ZgJointId, ZgJointId>>();
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftShoulder, ZgJointId.Torso));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightShoulder, ZgJointId.Torso));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftElbow, ZgJointId.LeftShoulder));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightElbow, ZgJointId.RightShoulder));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftHip, ZgJointId.Waist));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightHip, ZgJointId.Waist));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.LeftKnee, ZgJointId.LeftHip));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.RightKnee, ZgJointId.RightHip));
        relations.Add(new KeyValuePair<ZgJointId, ZgJointId>(ZgJointId.Neck, ZgJointId.Torso));
		
		
        foreach (KeyValuePair<ZgJointId, ZgJointId> e in relations)
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
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperArm, -90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightUpperLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(torso, 90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(waist, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(head, 90));

        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerLeg, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(leftLowerArm, -90));
        rotateMe.Add(new KeyValuePair<GameObject, float>(rightLowerArm, -90));

        foreach (KeyValuePair<GameObject, float> e in rotateMe)
        {
            GameObject tempParent = new GameObject("genTempParent");
            tempParent.transform.position = e.Key.transform.position;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < e.Key.transform.childCount; i++)
                if (e.Key.transform.GetChild(i).parent == e.Key.transform)
                    children.Add(e.Key.transform.GetChild(i));
            foreach (Transform f in children)
                f.parent = tempParent.transform;
            tempParent.transform.rotation = Quaternion.AngleAxis(e.Value, Vector3.forward) * tempParent.transform.rotation;
            foreach (Transform f in children)
                f.parent = e.Key.transform;
            GameObject.Destroy(tempParent);
        }
    }

    GameObject create_object(ZgJointId aId, Texture2D aTex, Texture2D aAttachTex)  
    {
        List<Vector3> attach = new List<Vector3>(){get_attachment_point(0, aAttachTex)};
        return create_object(aId, aTex, new Vector2(aTex.width,aTex.height), attach);
    }

    GameObject create_object(ZgJointId aId, Texture2D aTex, Vector2 aDim, List<Vector3> aAttach)
    {
        GameObject parent = new GameObject("genParent" + aId.ToString());
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.localScale = Vector3.one * 0.2f;
        //sphere.transform.parent = parent.transform;
        GameObject kid = (GameObject)GameObject.Instantiate(ManagerManager.Manager.mReferences.mPlanePrefab);
		//GameObject kid = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Plane); //TODO use prefab instead
        kid.renderer.material = new Material(ManagerManager.Manager.mReferences.mDefaultCharacterShader);
        kid.renderer.material.mainTexture = aTex;
        kid.transform.rotation = Quaternion.AngleAxis(90, Vector3.right) * kid.transform.rotation;

        kid.transform.localScale = new Vector3(BodyManager.convert_units(aDim.x) / 10.0f, 1, BodyManager.convert_units(aDim.y) / 10.0f) * GameConstants.SCALE;
        kid.transform.position = -aAttach[0]*GameConstants.SCALE;
        kid.transform.parent = parent.transform;

        mParts[aId] = parent;
        return parent;
    }
	
	
	public GameObject create_extremety(ZgJointId aId)
	{
		mParts[aId] = new GameObject("genExtremety");
		return mParts[aId];
	}


    



    //utilities for positioning
    Vector3 get_offset_of_plane(Transform aGo)
    {
		//NOTE this assumes plane is last child which it is...
        Transform plane = null;
		for(int i = 0; i < aGo.childCount; i++)
		{
			if(aGo.GetChild(i).gameObject.GetComponent<MeshFilter>() != null)
			{
				plane = aGo.GetChild(i);
				break;
			}
		}
        if (plane != null)
            return plane.position - aGo.transform.position;
		return Vector3.zero; //used for extremities
        //throw new UnityException("no plane child exsits");
    }
    static bool is_same_color(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
    static Vector3 index_to_position(int i, Texture2D aTex)
    {

        int x = i % aTex.width - aTex.width / 2;
        int y = i / aTex.width - aTex.height / 2;

        return new Vector3(-BodyManager.convert_units(x), BodyManager.convert_units(y));
    }
    public static Vector3 find_first_color(Color32 c, Texture2D aTex)
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
        throw new UnityException("color " + c.ToString() + " not found in texture " + aTex.name);
    }

    public static Vector3 get_attachment_point(int aId, Texture2D aTex)
    {
        Color32 c;
        switch (aId)
        {
            case 0:
                c = new Color32(255, 0, 0, 255);
                break;
            case 1:
                c = new Color32(0, 255, 0, 255);
                break;
            case 2:
                c = new Color32(0, 0, 255, 255);
                break;
            case 3:
                c = new Color32(255, 255, 0, 255);
                break;
            default:
                return Vector3.zero;
        }
        return find_first_color(c, aTex) * GameConstants.SCALE;
    }


    Vector3 get_connection_point_image(ZgJointId A, ZgJointId B, Texture2D aBTex)
    {
        if (A == B)
        {
            return get_attachment_point(0, aBTex);
        }
        if (B == ZgJointId.Waist)
        {
            if (A == ZgJointId.Torso)
                return get_attachment_point(0, aBTex);
            else if (A == ZgJointId.LeftHip)
                return get_attachment_point(1, aBTex);
            else if (A == ZgJointId.RightHip)
                return get_attachment_point(2, aBTex);
        }
        else if (B == ZgJointId.Torso)
        {
            if (A == ZgJointId.Neck)
                return get_attachment_point(3, aBTex);
            else if (A == ZgJointId.LeftShoulder)
                return get_attachment_point(1, aBTex);
            else if (A == ZgJointId.RightShoulder)
                return get_attachment_point(2, aBTex);
        }
        else
        {
            return get_attachment_point(1, aBTex);
        }
        throw new UnityException("uh oh, can't find attachment point zigjointid map");
    }

    Vector3 get_connection_point_list(ZgJointId A, ZgJointId B, List<Vector3> aConnect)
    {
        int index = -1;
        if (A == B)
        {
            index = 0;
        }
        if (B == ZgJointId.Waist)
        {
            if (A == ZgJointId.Torso)
                index = 0;
            else if (A == ZgJointId.LeftHip)
                index = 1;
            else if (A == ZgJointId.RightHip)
                index = 2;
        }
        else if (B == ZgJointId.Torso)
        {
            if (A == ZgJointId.Neck)
                index = 3;
            else if (A == ZgJointId.LeftShoulder)
                index = 1;
            else if (A == ZgJointId.RightShoulder)
                index = 2;
        }
        else
        {
            index = 1;
        }
        if(index == -1)
            throw new UnityException("uh oh, can't find attachment point zigjointid map");
        return aConnect[index]*GameConstants.SCALE;
    }

    public Vector3 get_Z_offset(ZgJointId id)
    {
        switch (id)
        {
            case ZgJointId.RightElbow:
                return new Vector3(0, 0, 0.9f);
            case ZgJointId.LeftElbow:
                return new Vector3(0, 0, 0.8f);
            case ZgJointId.RightShoulder:
                return new Vector3(0, 0, 0.7f);
            case ZgJointId.LeftShoulder:
                return new Vector3(0, 0, 0.6f);
			case ZgJointId.Neck:
				return new Vector3(0, 0, 0.55f);//lol
            case ZgJointId.Torso:
                return new Vector3(0, 0, 0.5f);
            case ZgJointId.Waist:
                return new Vector3(0, 0, 0.4f);
            case ZgJointId.LeftHip:
                return new Vector3(0, 0, 0.3f);
            case ZgJointId.RightHip:
                return new Vector3(0, 0, 0.2f);
            case ZgJointId.RightKnee:
                return new Vector3(0, 0, 0.1f);
            case ZgJointId.LeftKnee:
                return new Vector3(0, 0, 0.0f);
        }
        return Vector3.zero;
    }

	public Vector3 get_headfirst_Z_offset(ZgJointId id)
    {
        switch (id)
        {
			case ZgJointId.Neck:
				return new Vector3(0, 0, 0.9f);
			case ZgJointId.RightElbow:
				return new Vector3(0, 0, 0.8f);
			case ZgJointId.LeftElbow:
				return new Vector3(0, 0, 0.7f);
			case ZgJointId.RightShoulder:
				return new Vector3(0, 0, 0.6f);
			case ZgJointId.LeftShoulder:
				return new Vector3(0, 0, 0.55f);
            case ZgJointId.Torso:
                return new Vector3(0, 0, 0.5f);
            case ZgJointId.Waist:
                return new Vector3(0, 0, 0.4f);
            case ZgJointId.LeftHip:
                return new Vector3(0, 0, 0.3f);
            case ZgJointId.RightHip:
                return new Vector3(0, 0, 0.2f);
            case ZgJointId.RightKnee:
                return new Vector3(0, 0, 0.1f);
            case ZgJointId.LeftKnee:
                return new Vector3(0, 0, 0.0f);
        }
		return Vector3.zero;
	}
        
}


